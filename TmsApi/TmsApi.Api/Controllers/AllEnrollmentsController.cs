using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.DTOs;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/enrollments")]
[Tags("Enrollments")]
[Produces("application/json")]
[ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status500InternalServerError)]
public class AllEnrollmentsController(
    IEnrollmentService enrollmentService) : ControllerBase
{
    [HttpGet(Name = "ListEnrollments")]
    [ProducesResponseType(
        typeof(IReadOnlyList<EnrollmentResponseDto>),
        StatusCodes.Status200OK)]
    [EndpointSummary("List all course enrollments")]
    public async Task<IActionResult> GetAllEnrollments(CancellationToken ct)
    {
        var enrollments = await enrollmentService.GetAllAsync(ct);

        return Ok(enrollments);
    }

    [HttpPost("{id:int}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [EndpointSummary("Approve an enrollment")]
    public async Task<IActionResult> ApproveEnrollment(int id, CancellationToken ct)
    {
        var approved = await enrollmentService.ApproveAsync(id, ct);

        return approved ? NoContent() : NotFound();
    }
}