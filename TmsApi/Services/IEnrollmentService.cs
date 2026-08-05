namespace TmsApi.Services;
using Tms.Api.Dtos;
public interface IEnrollmentService
{
Task<List<EnrollmentResponseDto>> GetByCourseAsync(int courseId, CancellationToken ct);
Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken ct);
Task<EnrollmentResponseDto> CreateAsync(int courseId, EnrollStudentRequest request, CancellationToken ct);
}
