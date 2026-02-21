using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAttendance.API.Data;
using SchoolAttendance.API.Models;

namespace SchoolAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourseController : ControllerBase
{
    private readonly AppDbContext _context;

    public CourseController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
    {
        return await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Enrollments)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Course>> GetCourse(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        return course;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<Course>> CreateCourse(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> UpdateCourse(int id, Course course)
    {
        if (id != course.Id)
        {
            return BadRequest();
        }

        _context.Entry(course).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CourseExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/enroll")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult> EnrollStudent(int id, [FromBody] EnrollRequest request)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound(new { message = "Course not found" });
        }

        var student = await _context.Students.FindAsync(request.StudentId);
        if (student == null)
        {
            return NotFound(new { message = "Student not found" });
        }

        var existing = await _context.CourseEnrollments
            .FirstOrDefaultAsync(e => e.CourseId == id && e.StudentId == request.StudentId);

        if (existing != null)
        {
            return BadRequest(new { message = "Student already enrolled" });
        }

        var enrollment = new CourseEnrollment
        {
            CourseId = id,
            StudentId = request.StudentId
        };

        _context.CourseEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Student enrolled successfully" });
    }

    [HttpDelete("{id}/enroll/{studentId}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult> UnenrollStudent(int id, int studentId)
    {
        var enrollment = await _context.CourseEnrollments
            .FirstOrDefaultAsync(e => e.CourseId == id && e.StudentId == studentId);

        if (enrollment == null)
        {
            return NotFound();
        }

        _context.CourseEnrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/attendance")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult> MarkCourseAttendance(int id, [FromBody] CourseAttendanceRequest request)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound(new { message = "Course not found" });
        }

        var today = DateTime.UtcNow.Date;

        var existing = await _context.CourseAttendances
            .FirstOrDefaultAsync(a => a.CourseId == id && a.StudentId == request.StudentId && a.Date.Date == today);

        if (existing != null)
        {
            return BadRequest(new { message = "Attendance already marked for today" });
        }

        var attendance = new CourseAttendance
        {
            CourseId = id,
            StudentId = request.StudentId,
            Date = today,
            CheckInTime = DateTime.UtcNow,
            Status = request.Status,
            Notes = request.Notes ?? ""
        };

        _context.CourseAttendances.Add(attendance);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Attendance marked successfully" });
    }

    [HttpGet("{id}/attendance")]
    public async Task<ActionResult> GetCourseAttendance(int id, [FromQuery] DateTime? date)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;

        var attendance = await _context.CourseAttendances
            .Include(a => a.Student)
            .Where(a => a.CourseId == id && a.Date.Date == targetDate)
            .ToListAsync();

        var enrollments = await _context.CourseEnrollments
            .Include(e => e.Student)
            .Where(e => e.CourseId == id)
            .ToListAsync();

        return Ok(new
        {
            date = targetDate,
            totalEnrolled = enrollments.Count,
            attendanceRecords = attendance,
            presentCount = attendance.Count(a => a.Status == AttendanceStatus.Present),
            lateCount = attendance.Count(a => a.Status == AttendanceStatus.Late)
        });
    }

    private bool CourseExists(int id)
    {
        return _context.Courses.Any(e => e.Id == id);
    }
}

public class EnrollRequest
{
    public int StudentId { get; set; }
}

public class CourseAttendanceRequest
{
    public int StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}
