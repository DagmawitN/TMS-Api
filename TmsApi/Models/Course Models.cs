public class CourseModels
{
    public int Id { get; set; }
    public string Code {get; set;} = string.Empty;

    public string Title {get; set;} = string.Empty;

    public int Capacity {get; set;} = 0;
    public int EnrollementCount {get; set;} = 0;
    
}