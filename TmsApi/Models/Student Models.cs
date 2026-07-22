public class StudentModels
{
    public int Id {get; set;} = 0;
    public required string RegistrationNumber { get; set; } 
    public string Name {get; set;} = string.Empty;
    public int Age {get; set;} = 0;
    public decimal GPA {get; set;} = 0.0m;
    public bool IsActive { get; set; } = true;
}