namespace TmsApi.Infrastructure.Persistence;
using TmsApi.Application.DTOs;
public interface IEnrollmentService
{
Task<List<EnrollmentResponseDto>> GetAllAsync(CancellationToken ct);
Task<bool> ApproveAsync(int id, CancellationToken ct);
Task<List<EnrollmentResponseDto>> GetByCourseAsync(int courseId, CancellationToken ct);
Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken ct);
Task<EnrollmentResponseDto> CreateAsync(int courseId, EnrollStudentRequest request, CancellationToken ct);
}
