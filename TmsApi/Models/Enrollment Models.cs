public class EnrollmentModels
{
public int Id { get; set; } = 0;
public int StudentId { get; set; } = 0;
public int CourseId { get; set; } = 0;
public decimal? Grade { get; set; } // Nullable, as student may be currently enrolled
public DateTime ProcessedAt { get; set; }
}