public interface IEnrollmentService
{
    Task<EnrollmentModels> EnrollAsync(int studentId, string courseCode);
    Task<EnrollmentModels?> GetByIdAsync(string id);
    Task<IReadOnlyList<EnrollmentModels>> GetAllAsync();
    Task<bool> DeleteAsync(string id);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly Dictionary<string, EnrollmentModels> _store = new();
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(ILogger<EnrollmentService> logger)
    {
        _logger = logger;
    }

    public Task<EnrollmentModels> EnrollAsync(int studentId, string courseCode)
    {
        var id = Guid.NewGuid().ToString("N")[..8];

        var enrollment = new EnrollmentModels
        {
            Id = id,
            StudentId = studentId,
            CourseId = int.Parse(courseCode),
            ProcessedAt = DateTime.UtcNow
        };

        _store[id] = enrollment;

        _logger.LogInformation(
            "Enrolled {StudentId} in {CourseId} record {EnrollmentId}",
            studentId,
            courseCode,
            id);

        return Task.FromResult(enrollment);
    }

    public Task<EnrollmentModels?> GetByIdAsync(string id)
    {
        _store.TryGetValue(id, out var enrollment);
        return Task.FromResult(enrollment);
    }

    public Task<IReadOnlyList<EnrollmentModels>> GetAllAsync()
    {
        IReadOnlyList<EnrollmentModels> all = _store.Values.ToList();
        return Task.FromResult(all);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var removed = _store.Remove(id);
        return Task.FromResult(removed);
    }
}
public class TmsDatabaseException(string message) : Exception(message);

