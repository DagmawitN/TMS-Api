using Microsoft.AspNetCore.Mvc;
using TmsApi.Entities;
using TmsApi.Services;
using Tms.Api.Dtos;
namespace Tms.Api.Controllers;
[ApiController]
[Route("api/courses")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
[HttpGet("{id:int}", Name = nameof(GetCourseById))]
public async Task<IActionResult> GetCourseById(int id, CancellationToken ct)
{
var course = await courseService.GetByIdAsync(id, ct);
return course is not null ? Ok(course) : NotFound();}
[HttpPost]
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
