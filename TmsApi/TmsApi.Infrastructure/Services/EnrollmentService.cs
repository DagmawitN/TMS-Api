using Microsoft.EntityFrameworkCore;
using TmsApi.Domain.Entities;
using TmsApi.Application.DTOs;
using Microsoft.Extensions.Logging;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Application.Interfaces;

namespace TmsApi.Infrastructure.Persistence;

public class EnrollmentService(TmsDbContext context, ILogger<EnrollmentService> logger)
    : IEnrollmentService, TmsApi.Application.Interfaces.IEnrollmentService
{
    public Task<List<EnrollmentResponseDto>> GetAllAsync(CancellationToken ct) => context.Enrollments
        .AsNoTracking()
        .Select(e => new EnrollmentResponseDto(
            e.Id,
            e.CourseId,
            e.StudentId,
            e.EnrolledAt,
            e.Student.Name,
            e.Course.Title,
            e.Grade == null ? "Pending" : "Approved"))
        .ToListAsync(ct);

    public async Task<bool> ApproveAsync(int id, CancellationToken ct)
    {
        var enrollment = await context.Enrollments
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        if (enrollment is null)
        {
            return false;
        }

        enrollment.Grade ??= 0m;
        await context.SaveChangesAsync(ct);

        return true;
    }

    public async Task<List<EnrollmentResponseDto>> GetByCourseAsync(int courseId, CancellationToken ct) => await context.Enrollments
        .AsNoTracking()
        .Where(e => e.CourseId == courseId)
        .Select(e => new EnrollmentResponseDto(
            e.Id,
            e.CourseId,
            e.StudentId,
            e.EnrolledAt,
            e.Student.Name,
            e.Course.Title,
            e.Grade == null ? "Pending" : "Approved"))
        .ToListAsync(ct);

    public Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken ct) => context.Enrollments
        .AsNoTracking()
        .Where(e => e.Id == id && e.CourseId == courseId)
        .Select(e => new EnrollmentResponseDto(
            e.Id,
            e.CourseId,
            e.StudentId,
            e.EnrolledAt,
            e.Student.Name,
            e.Course.Title,
            e.Grade == null ? "Pending" : "Approved"))
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

    // Application-level methods (used by domain/application handlers)
    public Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct)
    {
        return context.Enrollments
            .Include(e => e.Course)
            .AnyAsync(e => e.StudentId == studentId && e.Course.Code == courseCode, ct);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct)
    {
        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync(ct);
    }

    public Task<List<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken ct)
    {
        return context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(ct);
    }
}
