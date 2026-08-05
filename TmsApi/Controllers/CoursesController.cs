using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TmsApi.Data;
using TmsApi.Services;
using Tms.Api.Dtos;
namespace Tms.Api.Controllers;
[ApiController]
[Route("api/courses")]
[Tags("Courses")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class CoursesController(ICourseService courseService, LinkGenerator linkGenerator) : ControllerBase
{
[HttpGet("{id:int}", Name = nameof(GetCourseById))]
[ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[EndpointSummary("Get a course by ID")]
[EndpointDescription("Returns course details with HATEOAS links. Returns 404 if the course does not exist.")]
public async Task<IActionResult> GetCourseById(int id, CancellationToken ct)
{
var course = await courseService.GetByIdAsync(id, ct);
if (course is null) return NotFound();
var coursePath = linkGenerator.GetPathByName(
    HttpContext, nameof(GetCourseById), new { id })!;
var enrollmentsPath = linkGenerator.GetPathByAction(
    HttpContext, action: "GetEnrollments", controller: "Enrollments",
    values: new { courseId = id })!;

var links = new List<LinkDto>
{
    new(coursePath, "self", "GET"),
    new(coursePath, "update", "PUT"),
    new(coursePath, "delete", "DELETE"),
    new(enrollmentsPath, "enrollments", "GET")
};

if (course.EnrollmentCount < course.MaxCapacity)
    links.Add(new(enrollmentsPath, "enroll", "POST"));

return Ok(new CourseDetailDto
{
    Id = course.Id,
    Code = course.Code,
    Title = course.Title,
    MaxCapacity = course.MaxCapacity,
    EnrollmentCount = course.EnrollmentCount,
    Links = links
});

}
[HttpGet]
[ProducesResponseType(typeof(PagedResponse<CourseResponseDto>), StatusCodes.Status200OK)]
[EndpointSummary("List courses with pagination")]
[EndpointDescription("Returns a paginated, optionally filtered listof TMS courses. PageSize is capped at 50.")]
public async Task<IActionResult> GetCourses(
[FromQuery] PagedRequest request, CancellationToken ct)
{
var result = await courseService.GetCoursesAsync(request, ct);
return Ok(result);
}
[HttpPost]
[ProducesResponseType(typeof(CourseResponseDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[EndpointSummary("Create a new course")]
[EndpointDescription("Creates a course with a unique code. Returns409 if the course code already exists.")]
public async Task<IActionResult> CreateCourse(CreateCourseRequest course, CancellationToken ct)
{
if (await courseService.CodeExistsAsync(course.Code, ct))
{
    return Conflict(new ProblemDetails
    {
        Title = "Course code already exists",
        Detail = $"A course with code '{course.Code}' is already registered.",
        Status = StatusCodes.Status409Conflict
    });
}

var result = await courseService.CreateAsync(course, ct);
// Return CreatedAtAction(nameof(GetCourseById), new {id = result.Id }, result).
// CreatedAtAction sets the Location header automatically.
return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, result);
}
}
