using Asp.Versioning;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Api.Filters;
using TmsApi.Middleware;
using TmsApi.Application.Enrollments.Commands;
using FluentValidation;
using MediatR;
using TmsApi.Api.ExceptionHandlers;
using TmsApi.Application.Behaviors;
using Microsoft.AspNetCore.Antiforgery;
using TmsApi.Hubs;
using TmsApi.Infrastructure.Services;
using TmsApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Tms.Api.Authorization;
using Microsoft.AspNetCore.Authorization;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi("v1", options =>
{
options.ShouldInclude = description =>
description.GroupName == "v1";
});
builder.Services.AddOpenApi("v2", options =>
{
options.ShouldInclude = description =>
description.GroupName == "v2";
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<TmsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TmsDatabase")
    ));

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{options.AddPolicy("AllowAngular", policy =>
policy.WithOrigins("http://localhost:4200")
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials());
});

builder.Services.AddIdentityCore<TmsUser>(options =>
{
// Enterprise Password Policy
options.Password.RequiredLength = 12;
options.Password.RequireUppercase = true;
options.Password.RequireDigit = true;
options.Password.RequireNonAlphanumeric = true;
// Brute-Force Lockout Protection
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
options.Lockout.AllowedForNewUsers = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<TmsDbContext>();


builder.Services.AddApiVersioning(options =>
{
options.DefaultApiVersion = new ApiVersion(1, 0);
options.AssumeDefaultVersionWhenUnspecified = true;
options.ReportApiVersions = true;
options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
options.GroupNameFormat = "'v'VVV";
options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddMediatR(cfg =>
cfg.RegisterServicesFromAssembly(typeof(EnrollStudentHandler).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(EnrollStudentValidator).Assembly);
builder.Services.AddControllers(options =>
{
options.Filters.Add<AuditLogFilter>();
});

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"];

builder.Services.AddCors(options =>
{
options.AddPolicy("TmsClient", policy =>
{
policy.WithOrigins(allowedOrigins)
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials() // Vital for HttpOnly auth cookies in Session 2
.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
});
});
builder.Services.AddAntiforgery(options =>
{
options.HeaderName = "X-XSRF-TOKEN";
});



builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped <ICourseService, CourseService>();
builder.Services.AddScoped <IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<TmsApi.Application.Interfaces.IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<TmsApi.Application.Interfaces.ICourseService, CourseService>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<TokenService>();
builder.Services.AddAuthentication(options =>
{
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuer = true,
ValidateAudience = true,
ValidateLifetime = true,
ValidateIssuerSigningKey = true,
ValidIssuer = builder.Configuration["Jwt:Issuer"],
ValidAudience = builder.Configuration["Jwt:Audience"],
IssuerSigningKey = new SymmetricSecurityKey(
Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
};
});

builder.Services.AddAuthorizationBuilder()
.AddPolicy("CanEditCourse", policy =>
policy.Requirements.Add(new CourseInstructorRequirement()));
builder.Services.AddSingleton<IAuthorizationHandler, CourseInstructorHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseStatusCodePages();

// app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseRouting();
app.UseCors("TmsClient");
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
if (context.User.Identity?.IsAuthenticated == true || context.
Request.Cookies.ContainsKey("tms_auth"))
{
var antiforgery = context.RequestServices
.GetRequiredService<IAntiforgery>();
var tokens = antiforgery.GetAndStoreTokens(context);
context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
new CookieOptions
{
HttpOnly = false, // MUST be false so Angular JavaScript can read it!
Secure = !builder.Environment.IsDevelopment(),
SameSite = SameSiteMode.Strict
});
}
await next(context);
});
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append(
    "Content-Security-Policy",
    "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';");
    await next(context);
});
app.UseMiddleware<V1DeprecationMiddleware>();
app.MapControllers();
app.MapHub<TmsHub>("/hubs/tms").RequireCors("TmsClient");




if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("TMS API Reference")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp,
                ScalarClient.HttpClient);
        // Tell Scalar to pull both documents into its sidebar dropdown
        options
            .AddDocument("v1", "API Version 1.0")
            .AddDocument("v2", "API Version 2.0");
    });
}



// Removed calls to Map/UseMapScalarApiReference - extension not available in this project
// app.MapGet("/api/error", () =>
// {
// throw new TmsDatabaseException("Simulated database failure for ProblemDetails testing");
// });

// using (var scope = app.Services.CreateScope())
// {
// var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();
// //context.Database.Migrate(); // Applies any pending migrations; keeps migration history intact
// if (!context.Students.Any())
// {
// var students = new List<Student>
//     {
//         new() { RegistrationNumber = "TMS-2026-0001", Name = "AliceSmith", GPA = 3.8m, IsActive = true },
//         new() { RegistrationNumber = "TMS-2026-0002", Name = "Bob Jones", GPA = 2.9m, IsActive = true },
//         new() { RegistrationNumber = "TMS-2026-0003", Name = "Charlie Brown", GPA = 3.4m, IsActive = false },
//         new() { RegistrationNumber = "TMS-2026-0004", Name = "Diana Prince", GPA = 3.9m, IsActive = true },
//         new() { RegistrationNumber = "TMS-2026-0005", Name = "Evan Wright", GPA = 2.5m, IsActive = true }
//     };
// context.Students.AddRange(students);
// var courses = new List<Course>
// {
// new() { Code = "CS-101", Title = "Introduction to ComputerScience", Capacity = 30 },
// new() { Code = "CS-201", Title = "Data Structures and Algorithms", Capacity = 25 },
// new() { Code = "MAT-101", Title = "Calculus I", Capacity =
// 40 }
// };
// context.Courses.AddRange(courses);
// context.SaveChanges();
// var enrollments = new List<Enrollment>
// {
// new() { StudentId = students[0].Id, CourseId = courses[0].Id, Grade = 4.0m },
// new() { StudentId = students[0].Id, CourseId = courses[1].Id, Grade = 3.6m },
// new() { StudentId = students[1].Id, CourseId = courses[0].Id, Grade = 2.8m },
// new() { StudentId = students[3].Id, CourseId = courses[1].Id, Grade = 3.9m }
// };
// context.Enrollments.AddRange(enrollments);
// context.SaveChanges();
// }
// }

var service = new CryptoDemoService();

string hash1 = service.HashUserPassword("Password123!");
string hash2 = service.HashUserPassword("Password123!");

Console.WriteLine($"Hash 1: {hash1}");
Console.WriteLine($"Hash 2: {hash2}");

bool match1 = service.VerifyUserPassword("Password123!", hash1);
bool match2 = service.VerifyUserPassword("Password123!", hash2);

Console.WriteLine($"Match 1: {match1}");
Console.WriteLine($"Match 2: {match2}");

if (app.Environment.IsDevelopment())
{
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();

}

app.Run();

