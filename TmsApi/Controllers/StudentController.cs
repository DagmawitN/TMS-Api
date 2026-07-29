using Microsoft.AspNetCore.Mvc;
using TmsApi.Data;
using TmsApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentController : ControllerBase
{
    private readonly TmsDbContext _context;

    public StudentController(TmsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents(int page = 1)
    {
        const int pageSize = 20;

        var students = await _context.Students
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(students);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(int id, Student updatedStudent)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return NotFound();

        student.Name = updatedStudent.Name;
        student.GPA = updatedStudent.GPA;

        // Set the shadow property
        _context.Entry(student)
            .Property("LastUpdated")
            .CurrentValue = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}

