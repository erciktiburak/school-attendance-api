using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAttendance.API.Data;
using SchoolAttendance.API.Models;

namespace SchoolAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Teacher,Admin")]
public class TeacherController : ControllerBase
{
    private readonly AppDbContext _context;

    public TeacherController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> GetTeacherDashboard()
    {
        var today = DateTime.UtcNow.Date;

        var totalStudents = await _context.Students.CountAsync();
        var todayAttendance = await _context.AttendanceRecords
            .Where(a => a.CheckInTime.Date == today)
            .CountAsync();

        var lateStudents = await _context.AttendanceRecords
            .Include(a => a.Student)
            .Where(a => a.CheckInTime.Date == today && a.Status == AttendanceStatus.Late)
            .ToListAsync();

        var absentStudents = await _context.Students
            .Where(s => !_context.AttendanceRecords
                .Any(a => a.StudentId == s.Id && a.CheckInTime.Date == today))
            .ToListAsync();

        return Ok(new
        {
            totalStudents,
            todayAttendance,
            lateCount = lateStudents.Count,
            absentCount = absentStudents.Count,
            lateStudents,
            absentStudents,
            date = today
        });
    }

    [HttpGet("students/at-risk")]
    public async Task<ActionResult> GetAtRiskStudents()
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        var students = await _context.Students
            .Select(s => new
            {
                s.Id,
                s.StudentNumber,
                s.FirstName,
                s.LastName,
                s.Department,
                s.Email,
                AttendanceCount = _context.AttendanceRecords
                    .Count(a => a.StudentId == s.Id && a.CheckInTime >= thirtyDaysAgo),
                LateCount = _context.AttendanceRecords
                    .Count(a => a.StudentId == s.Id && a.CheckInTime >= thirtyDaysAgo && a.Status == AttendanceStatus.Late)
            })
            .ToListAsync();

        var atRiskStudents = students
            .Where(s => s.AttendanceCount < 15 || s.LateCount > 5)
            .OrderBy(s => s.AttendanceCount)
            .ToList();

        return Ok(atRiskStudents);
    }

    [HttpPost("mark-absent")]
    public async Task<ActionResult> MarkStudentAbsent([FromBody] MarkAbsentRequest request)
    {
        var student = await _context.Students.FindAsync(request.StudentId);
        if (student == null)
        {
            return NotFound(new { message = "Student not found" });
        }

        var today = DateTime.UtcNow.Date;
        var existingRecord = await _context.AttendanceRecords
            .FirstOrDefaultAsync(a => a.StudentId == request.StudentId && a.CheckInTime.Date == today);

        if (existingRecord != null)
        {
            return BadRequest(new { message = "Student already has attendance record for today" });
        }

        var record = new AttendanceRecord
        {
            StudentId = request.StudentId,
            CheckInTime = DateTime.UtcNow,
            Status = request.IsExcused ? AttendanceStatus.Excused : AttendanceStatus.Absent,
            Location = "Marked by teacher",
        };

        _context.AttendanceRecords.Add(record);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Student marked as absent", record });
    }

    [HttpGet("daily-summary")]
    public async Task<ActionResult> GetDailySummary([FromQuery] DateTime? date)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;

        var records = await _context.AttendanceRecords
            .Include(a => a.Student)
            .Where(a => a.CheckInTime.Date == targetDate)
            .ToListAsync();

        var totalStudents = await _context.Students.CountAsync();
        var presentCount = records.Count(r => r.Status == AttendanceStatus.Present);
        var lateCount = records.Count(r => r.Status == AttendanceStatus.Late);
        var absentCount = totalStudents - records.Count;

        var hourlyBreakdown = records
            .GroupBy(r => r.CheckInTime.Hour)
            .Select(g => new
            {
                hour = g.Key,
                count = g.Count()
            })
            .OrderBy(x => x.hour)
            .ToList();

        return Ok(new
        {
            date = targetDate,
            totalStudents,
            presentCount,
            lateCount,
            absentCount,
            attendanceRate = totalStudents > 0 ? Math.Round((double)presentCount / totalStudents * 100, 2) : 0,
            hourlyBreakdown,
            recentCheckIns = records.OrderByDescending(r => r.CheckInTime).Take(10)
        });
    }
}

public class MarkAbsentRequest
{
    public int StudentId { get; set; }
    public bool IsExcused { get; set; }
}
