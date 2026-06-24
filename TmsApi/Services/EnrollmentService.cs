public interface IEnrollmentService
{
    Task<Enrollment> EnrollAsync(string studentId, string courseCode);
    Task<Enrollment?> GetByIdAsync(string id);
    Task<IReadOnlyList<Enrollment>> GetAllAsync();
    Task<bool> DeleteAsync(string id);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly Dictionary<string, Enrollment> _store = new();
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(ILogger<EnrollmentService> logger)
    {
        _logger = logger;
    }

    public Task<Enrollment> EnrollAsync(string studentId, string courseCode)
    {
        var id = Guid.NewGuid().ToString("N")[..8];

        var enrollment = new Enrollment
        {
            Id = id,
            StudentId = studentId,
            CourseCode = courseCode,
            ProcessedAt = DateTime.UtcNow
        };

        _store[id] = enrollment;

        _logger.LogInformation(
            "Enrolled {StudentId} in {CourseCode} record {EnrollmentId}",
            studentId,
            courseCode,
            id);

        return Task.FromResult(enrollment);
    }

    public Task<Enrollment?> GetByIdAsync(string id)
    {
        _store.TryGetValue(id, out var enrollment);
        return Task.FromResult(enrollment);
    }

    public Task<IReadOnlyList<Enrollment>> GetAllAsync()
    {
        IReadOnlyList<Enrollment> all = _store.Values.ToList();
        return Task.FromResult(all);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var removed = _store.Remove(id);
        return Task.FromResult(removed);
    }
}