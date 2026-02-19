using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAttendance.API.Data;
using SchoolAttendance.API.Models;

namespace SchoolAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatisticsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/statistics/dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboardStats()
    {
        var today = DateTime.UtcNow.Date;
        
        var totalStudents = await _context.Students.CountAsync();
        var todayAttendance = await _context.AttendanceRecords
            .Where(a => a.CheckInTime.Date == today)
            .CountAsync();
        
        var activeCheckIns = await _context.AttendanceRecords
            .Where(a => a.CheckInTime.Date == today && a.CheckOutTime == null)
            .CountAsync();

        var attendanceRate = totalStudents > 0 
            ? Math.Round((double)todayAttendance / totalStudents * 100, 2) 
            : 0;

        return Ok(new
        {
            totalStudents = totalStudents,
            todayAttendance = todayAttendance,
            activeCheckIns = activeCheckIns,
            attendanceRate = attendanceRate,
            date = today
        });
    }

    // GET: api/statistics/weekly
    [HttpGet("weekly")]
    public async Task<ActionResult> GetWeeklyStats()
    {
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        
        var weeklyData = await _context.AttendanceRecords
            .Where(a => a.CheckInTime.Date >= weekStart && a.CheckInTime.Date <= today)
            .GroupBy(a => a.CheckInTime.Date)
            .Select(g => new
            {
                date = g.Key,
                count = g.Count(),
                present = g.Count(a => a.Status == AttendanceStatus.Present),
                late = g.Count(a => a.Status == AttendanceStatus.Late)
            })
            .OrderBy(x => x.date)
            .ToListAsync();

        return Ok(new
        {
            weekStart = weekStart,
            weekEnd = today,
            dailyStats = weeklyData,
            totalAttendance = weeklyData.Sum(d => d.count)
        });
    }

    // GET: api/statistics/monthly
    [HttpGet("monthly")]
    public async Task<ActionResult> GetMonthlyStats()
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        
        var monthlyData = await _context.AttendanceRecords
            .Where(a => a.CheckInTime.Date >= monthStart && a.CheckInTime.Date <= today)
            .GroupBy(a => a.CheckInTime.Date)
            .Select(g => new
            {
                date = g.Key,
                count = g.Count(),
                present = g.Count(a => a.Status == AttendanceStatus.Present),
                late = g.Count(a => a.Status == AttendanceStatus.Late)
            })
            .OrderBy(x => x.date)
            .ToListAsync();

        var totalStudents = await _context.Students.CountAsync();
        var uniqueAttendees = await _context.AttendanceRecords
            .Where(a => a.CheckInTime.Date >= monthStart && a.CheckInTime.Date <= today)
            .Select(a => a.StudentId)
            .Distinct()
            .CountAsync();

        return Ok(new
        {
            month = monthStart.ToString("MMMM yyyy"),
            monthStart = monthStart,
            monthEnd = today,
            totalStudents = totalStudents,
            uniqueAttendees = uniqueAttendees,
            participationRate = totalStudents > 0 
                ? Math.Round((double)uniqueAttendees / totalStudents * 100, 2) 
                : 0,
            dailyStats = monthlyData,
            totalAttendance = monthlyData.Sum(d => d.count)
        });
    }

    // GET: api/statistics/student/{studentId}
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult> GetStudentStats(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
        {
            return NotFound(new { message = "Student not found" });
        }

        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var totalAttendance = await _context.AttendanceRecords
            .Where(a => a.StudentId == studentId)
            .CountAsync();

        var monthlyAttendance = await _context.AttendanceRecords
            .Where(a => a.StudentId == studentId && 
                        a.CheckInTime.Date >= monthStart && 
                        a.CheckInTime.Date <= today)
            .CountAsync();

        var presentCount = await _context.AttendanceRecords
            .Where(a => a.StudentId == studentId && a.Status == AttendanceStatus.Present)
            .CountAsync();

        var lateCount = await _context.AttendanceRecords
            .Where(a => a.StudentId == studentId && a.Status == AttendanceStatus.Late)
            .CountAsync();

        var averageDuration = await _context.AttendanceRecords
            .Where(a => a.StudentId == studentId && a.CheckOutTime != null)
            .Select(a => (a.CheckOutTime!.Value - a.CheckInTime).TotalMinutes)
            .DefaultIfEmpty(0)
            .AverageAsync();

        return Ok(new
        {
            student = new
            {
                id = student.Id,
                name = $"{student.FirstName} {student.LastName}",
                studentNumber = student.StudentNumber,
                department = student.Department
            },
            totalAttendance = totalAttendance,
            monthlyAttendance = monthlyAttendance,
            presentCount = presentCount,
            lateCount = lateCount,
            attendanceRate = totalAttendance > 0 
                ? Math.Round((double)presentCount / totalAttendance * 100, 2) 
                : 0,
            averageDurationMinutes = Math.Round(averageDuration, 2)
        });
    }

    // GET: api/statistics/department/{department}
    [HttpGet("department/{department}")]
    public async Task<ActionResult> GetDepartmentStats(string department)
    {
        var today = DateTime.UtcNow.Date;
        
        var students = await _context.Students
            .Where(s => s.Department == department)
            .ToListAsync();

        if (!students.Any())
        {
            return NotFound(new { message = "No students found in this department" });
        }

        var studentIds = students.Select(s => s.Id).ToList();
        
        var todayAttendance = await _context.AttendanceRecords
            .Where(a => studentIds.Contains(a.StudentId) && a.CheckInTime.Date == today)
            .CountAsync();

        var totalAttendance = await _context.AttendanceRecords
            .Where(a => studentIds.Contains(a.StudentId))
            .CountAsync();

        return Ok(new
        {
            department = department,
            totalStudents = students.Count,
            todayAttendance = todayAttendance,
            totalAttendance = totalAttendance,
            attendanceRate = students.Count > 0 
                ? Math.Round((double)todayAttendance / students.Count * 100, 2) 
                : 0
        });
    }
}