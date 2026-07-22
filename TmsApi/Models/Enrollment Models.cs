public class EnrollmentModels
{
public string Id { get; set; } = string.Empty;
public int StudentId { get; set; } = 0;
public int CourseId { get; set; }
public decimal? Grade { get; set; } // Nullable, as student may be currently enrolled
public DateTime ProcessedAt { get; set; }
}