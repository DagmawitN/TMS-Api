using System;

public class EnrollmentService
{
    public EnrollmentRecord ProcessRegistration(Student? student, Course? course)
    {
        // Guard clauses
        if (student is null)
            throw new ArgumentNullException(nameof(student));

        if (course is null)
            throw new ArgumentNullException(nameof(course));

        // Business rule
        if (course.EnrolledCount >= course.Capacity)
            throw new CapacityReachedException(course.Code);

        // GPA Classification
        string standing = student.GPA switch
        {
            >= 3.5m => "Honors",
            >= 2.5m => "Good Standing",
            _ => "Academic Warning"
        };

        Console.WriteLine($"{student.Name} is in {standing}.");

        return new EnrollmentRecord(
            student.Id,
            course.Code,
            DateTime.UtcNow
        );
    }

    public async Task<Student> FetchStudentAsync(string id)
    {
        try
        {
            Console.WriteLine($"Fetching {id}...");

            await Task.Delay(300);

            return new Student
            {
                Id = id,
                Name = $"Student-{id}",
                Age = 20,
                GPA = id switch
                {
                    "S1" => 3.8m,
                    "S2" => 2.4m,
                    "S3" => 3.5m,
                    "S4" => 1.9m,
                    "S5" => 3.2m,
                    _ => 2.5m
                }
            };
        }
        catch (Exception ex)
        {
            throw new TmsDatabaseException(
                "Load Student",
                $"Unable to load student {id}.",
                ex);
        }
    }

    public async Task<Course> FetchCourseAsync(string code)
    {
        try
        {
            Console.WriteLine($"Fetching course {code}...");

            await Task.Delay(200);

            return new Course
            {
                Code = code,
                Title = $"Course-{code}",
                Capacity = code switch
                {
                    "CRS-101" => 2,
                    "CRS-201" => 30,
                    "CRS-301" => 15,
                    _ => 25
                }
            };
        }
        catch (Exception ex)
        {
            throw new TmsDatabaseException(
                "Load Course",
                $"Unable to load course {code}.",
                ex);
        }
    }

    public async Task SendConfirmationAsync(Student student)
    {
        try
        {
            await Task.Delay(100);

            Console.WriteLine($"Email sent to {student.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email failed for {student.Name}: {ex.Message}");
        }
    }
}