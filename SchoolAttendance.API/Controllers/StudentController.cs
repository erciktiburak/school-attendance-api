using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAttendance.API.Data;
using SchoolAttendance.API.Models;
using SchoolAttendance.API.Services;

namespace SchoolAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/student
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
    {
        return await _context.Students.ToListAsync();
    }

    // GET: api/student/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        return student;
    }

    // GET: api/student/qr/{qrCode}
    [HttpGet("qr/{qrCode}")]
    public async Task<ActionResult<Student>> GetStudentByQR(string qrCode)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.QRCode == qrCode);

        if (student == null)
        {
            return NotFound(new { message = "Student not found with this QR code" });
        }

        return student;
    }

    // POST: api/student
    [HttpPost]
    public async Task<ActionResult<Student>> CreateStudent(Student student)
    {
        // Generate unique QR code
        student.QRCode = Guid.NewGuid().ToString();
        student.CreatedAt = DateTime.UtcNow;

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }

    // PUT: api/student/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(int id, Student student)
    {
        if (id != student.Id)
        {
            return BadRequest();
        }

        _context.Entry(student).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StudentExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/student/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.Id == id);
    }

    // GET: api/student/5/qrcode
    [HttpGet("{id}/qrcode")]
    public async Task<IActionResult> GetStudentQRCode(int id, [FromServices] IQRCodeService qrCodeService)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var qrBytes = qrCodeService.GenerateQRCode(student.QRCode);
        return File(qrBytes, "image/png");
    }

    // GET: api/student/5/qrcode/base64
    [HttpGet("{id}/qrcode/base64")]
    public async Task<ActionResult> GetStudentQRCodeBase64(int id, [FromServices] IQRCodeService qrCodeService)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var qrBase64 = qrCodeService.GenerateQRCodeBase64(student.QRCode);
        
        return Ok(new
        {
            studentId = student.Id,
            studentName = $"{student.FirstName} {student.LastName}",
            qrCodeBase64 = $"data:image/png;base64,{qrBase64}"
        });
    }
}