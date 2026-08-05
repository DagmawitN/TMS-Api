using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
using Tms.Api.Dtos;
using TmsApi.Services;
public class EnrollmentService(TmsDbContext context, ILogger<EnrollmentService> logger) : IEnrollmentService
{
public async Task<List<EnrollmentResponseDto>> GetByCourseAsync(int courseId, CancellationToken ct) => await context.Enrollments
.AsNoTracking()
.Where(e => e.CourseId == courseId)
.Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.StudentId, e.EnrolledAt))
.ToListAsync(ct);

public Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken ct) => context.Enrollments
.AsNoTracking()
.Where(e => e.Id == id && e.CourseId == courseId)
.Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.
StudentId, e.EnrolledAt))
.FirstOrDefaultAsync(ct);
public async Task<EnrollmentResponseDto> CreateAsync(int courseId, EnrollStudentRequest request, CancellationToken ct)
{
var enrollment = new Enrollment
{
CourseId = courseId,
StudentId = request.StudentId,
EnrolledAt = DateTime.UtcNow
};

context.Enrollments.Add(enrollment);
await context.SaveChangesAsync(ct);

logger.LogInformation(
    "Created enrollment {EnrollmentId} for student {StudentId} in course {CourseId}",
    enrollment.Id,
    enrollment.StudentId,
    enrollment.CourseId);

return (await GetByIdAsync(courseId, enrollment.Id, ct))!;
}
}
