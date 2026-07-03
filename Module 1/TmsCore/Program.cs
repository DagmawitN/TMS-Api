// string studentName = "Abeba";
// string studentId = "STU-001";
// int enrollmentCount = 3;
// decimal grantPerStudent = 1999.99m;
// decimal totalAllocation = grantPerStudent * 100_000m;
// DateTime enrolledAt = DateTime.UtcNow;
// string? campusRegion = null;
// Console.WriteLine($"Student: {studentName} ({studentId})");
// Console.WriteLine($"Courses: {enrollmentCount}");
// //Console.WriteLine($"Grant: {grantAmount:F2}");
// Console.WriteLine($"Enrolled: {enrolledAt:yyyy-MM-dd}");
// Console.WriteLine($"Campus: {campusRegion ?? "Not assigned"}");
// //Console.WriteLine($"Total allocated (decimal): {totalAllocation:F2}");
// Console.WriteLine($"Total allocated (decimal): {totalAllocation}");
// Console.WriteLine($"Total allocated (formatted): {totalAllocation:F2}");

using System.Diagnostics;
// C# 12+ Collection Expressions the modern way to initialize lists
List<Student> studentList = [
new Student { Id = "S1", Name = "Abeba", Age = 22, GPA = 3.8m }, new Student { Id = "S2", Name = "Kidane", Age = 21, GPA = 2.4m }, new Student { Id = "S3", Name = "Dawit", Age = 20, GPA = 3.1m }, new Student { Id = "S4", Name = "Sara", Age = 23, GPA = 3.9m }, new Student { Id = "S5", Name = "Frehiwot", Age = 19, GPA = 2.0m }, new Student { Id = "S6", Name = "Yonas", Age = 24, GPA = 3.5m }, new Student { Id = "S7", Name = "Meron", Age = 22, GPA = 1.8m }, new Student { Id = "S8", Name = "Tesfaye", Age = 21, GPA = 2.9m }
];
var enrollment = new EnrollmentRecord("STU-001", "CS-401", DateTime.UtcNow);
Console.WriteLine(enrollment);
var corrected = enrollment with { CourseCode = "CS-402" };
Console.WriteLine(corrected);
// Value equality — two records with the same data are equal
var duplicate = new EnrollmentRecord("STU-001", "CS-401", enrollment.EnrolledAt);
Console.WriteLine($"Same data? {enrollment == duplicate}");

var course = new Course { Code = "CS-401", Title = "Advanced C#", Capacity = 30 };
Console.WriteLine($"Course: {course.Title} (Capacity: {course.Capacity})");
// Invalid capacity — should throw
try
{
course.Capacity = -5;
}
catch (ArgumentOutOfRangeException ex)
{
Console.WriteLine($"Caught: {ex.Message}");
}
// Invalid title — should throw
try
{
course.Title = "";
}
catch (ArgumentException ex)
{
Console.WriteLine($"Caught: {ex.Message}");
}

var s = new Student { Id = "S1", Name = "Abeba", Age = 20, GPA = 3.8m };
Console.WriteLine($"Student: {s.Name}, GPA: {s.GPA}");

void PrintGradeReport(IEnumerable<IGradable> assessments)
{
Console.WriteLine("--- Grade Report ---");
foreach (var item in assessments)
{
Console.WriteLine($"{item.Title}: {item.CalculateGrade():F2}%");
}
}
// Test it — one array holds two completely different types
IGradable[] cohortAssessments = [
new Quiz { Title = "C# Basics", CorrectAnswers = 18, TotalQuestions = 20 }, new LabAssignment { Title = "Registration API", FunctionalityScore = 90m, CodeQualityScore = 85m }
];
PrintGradeReport(cohortAssessments);

var service = new EnrollmentService();
// Test 1: Valid registration
var validStudent = new Student { Id = "S1", Name = "Abeba", Age = 20, GPA = 3.8m };
var validCourse = new Course { Code = "CS-401", Title = "Advanced C#", Capacity = 30 };
var result = service.ProcessRegistration(validStudent, validCourse);
Console.WriteLine($"Enrolled: {result.StudentId} in {result.CourseCode}");
// Test 2: Null student should throw
try
{
service.ProcessRegistration(null, validCourse);
}
catch (ArgumentNullException ex)
{
Console.WriteLine($"Guard caught: {ex.ParamName}");
}
// Test 3: Full course should throw
var fullCourse = new Course { Code = "CS-402", Title = "Full Course", Capacity = 1 };
fullCourse.EnrolledCount = 1;
try
{
service.ProcessRegistration(validStudent, fullCourse);
}
catch (InvalidOperationException ex)
{
Console.WriteLine($"Business rule: {ex.Message}");
}

var sw = Stopwatch.StartNew();
for (int i = 0; i < 5; i++)
{
Thread.Sleep(300); // Thread is HELD for 300ms cannot serve anyone else
}
Console.WriteLine($"Blocking sequential: {sw.ElapsedMilliseconds}ms");
// ASYNC BUT STILL SEQUENTIAL: Thread released, but calls are one-at-a-time
sw.Restart();
for (int i = 0; i < 5; i++)
{
await Task.Delay(300); // Thread released while waiting but still sequential
}
Console.WriteLine($"Async sequential: {sw.ElapsedMilliseconds}ms");
// THE RIGHT WAY: Async parallel all 5 start simultaneously
sw.Restart();
var tasks = Enumerable.Range(0, 5).Select(_ => Task.Delay(300));
await Task.WhenAll(tasks);
Console.WriteLine($"Async parallel: {sw.ElapsedMilliseconds}ms");

async Task<Student> FetchStudentAsync(string id)
{
Console.WriteLine($" Fetching {id}...");
await Task.Delay(300); // Simulate database latency
return new Student
{
Id = id,
Name = $"Student-{id}", Age = 20, GPA = id switch
{ "S1" => 3.8m, "S2" => 2.4m, "S3" => 3.5m, "S4" => 1.9m, "S5" => 3.2m, _ => 2.5m
}
};
}
async Task<Course> FetchCourseAsync(string code)
{
Console.WriteLine($" Fetching course {code}...");
await Task.Delay(200); // Simulate database latency
return new Course
{
Code = code, Title = $"Course-{code}", Capacity = code switch
{ "CRS-101" => 2, "CRS-201" => 30, "CRS-301" => 15, _ => 25
}
};
}
sw.Restart();
// Start all fetches simultaneously students AND courses
string[] studentIds = ["S1", "S2", "S3", "S4", "S5"];
string[] courseCodes = ["CRS-101", "CRS-201", "CRS-301"];
var studentTasks = studentIds.Select(id => FetchStudentAsync(id));
var courseTasks = courseCodes.Select(code => FetchCourseAsync(code));
// Both arrays load concurrently
Student[] students = await Task.WhenAll(studentTasks);
Course[] courses = await Task.WhenAll(courseTasks);
Console.WriteLine($"\nLoaded {students.Length} students and {courses.Length} courses in {sw.ElapsedMilliseconds}ms");
foreach (var st in students)
{
Console.WriteLine($" {st.Name} GPA: {st.GPA}");
}
var enrollCourse = new Course { Code = "CRS-101", Title = "C# Mastery", Capacity = 2 };
var enrollService = new EnrollmentService();
var enrollments = new List<EnrollmentRecord>();
var failures = new List<string>();
sw.Restart();
foreach (var student in students)
{
try
{
var record = enrollService.ProcessRegistration(student, enrollCourse);
enrollCourse.EnrolledCount++;
enrollments.Add(record);
Console.WriteLine($" Enrolled: {student.Name}");
}
catch (InvalidOperationException ex)
{
failures.Add($"{student.Name}: {ex.Message}");
Console.WriteLine($" Rejected: {student.Name} {ex.Message}");
}
}
async Task SendConfirmationAsync(Student student)
{
try
{
await Task.Delay(100); // Simulate sending email
Console.WriteLine($" Email sent to {student.Name}");
}
catch (Exception ex)
{
// Log the failure do NOT re-throw. // This is intentional fire-and-forget. 
Console.WriteLine($" Email failed for {student.Name}: {ex.Message}");
}
}
try
{
var overflowCourse = new Course { Code = "CRS-999", Title = "Overflow Test", Capacity = 0 };
enrollService.ProcessRegistration(
new Student { Id = "S99", Name = "Test", Age = 20, GPA = 3.0m }, overflowCourse
);
}
catch (CapacityReachedException ex)
{
Console.WriteLine($"\nDomain exception caught:");
Console.WriteLine($" Course: {ex.CourseCode}");
Console.WriteLine($" Message: {ex.Message}");
}
sw.Stop();
// Calculate class average GPA from loaded students
decimal classAverage = students.Length > 0
? students.Average(s => s.GPA)
: 0m;
// Print the final report
Console.WriteLine("\n========== ENROLLMENT SUMMARY ==========");
Console.WriteLine($"Total students loaded: {students.Length}");
Console.WriteLine($"Successful enrollments: {enrollments.Count}");
Console.WriteLine($"Failed enrollments: {failures.Count}");
Console.WriteLine($"Class average GPA: {classAverage:F2}");
Console.WriteLine($"Total elapsed time: {sw.ElapsedMilliseconds}ms");
if (failures.Count > 0)
{
Console.WriteLine("\n--- Failure Details ---");
foreach (var failure in failures)
{
Console.WriteLine($" {failure}");
}
}
Console.WriteLine("========================================");