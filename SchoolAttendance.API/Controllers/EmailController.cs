using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAttendance.API.Data;
using SchoolAttendance.API.Models;
using SchoolAttendance.API.Services;

namespace SchoolAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Teacher")]
public class EmailController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public EmailController(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("send-absent-notification")]
    public async Task<ActionResult> SendAbsentNotification([FromBody] AbsentNotificationRequest request)
    {
        var student = await _context.Students.FindAsync(request.StudentId);
        if (student == null)
        {
            return NotFound(new { message = "Student not found" });
        }

        try
        {
            await _emailService.SendAbsentNotificationAsync(
                student.Email,
                $"{student.FirstName} {student.LastName}",
                request.Date
            );

            return Ok(new { message = "Notification sent successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to send email: {ex.Message}" });
        }
    }

    [HttpPost("send-late-notification")]
    public async Task<ActionResult> SendLateNotification([FromBody] int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
        {
            return NotFound(new { message = "Student not found" });
        }

        var latestAttendance = await _context.AttendanceRecords
            .Where(a => a.StudentId == studentId && a.Status == AttendanceStatus.Late)
            .OrderByDescending(a => a.CheckInTime)
            .FirstOrDefaultAsync();

        if (latestAttendance == null)
        {
            return NotFound(new { message = "No late attendance found" });
        }

        try
        {
            await _emailService.SendLateNotificationAsync(
                student.Email,
                $"{student.FirstName} {student.LastName}",
                latestAttendance.CheckInTime
            );

            return Ok(new { message = "Notification sent successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to send email: {ex.Message}" });
        }
    }

    [HttpPost("send-weekly-report")]
    public async Task<ActionResult> SendWeeklyReport([FromBody] int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
        {
            return NotFound(new { message = "Student not found" });
        }

        var weekAgo = DateTime.UtcNow.AddDays(-7);
        var weeklyRecords = await _context.AttendanceRecords
            .Where(a => a.StudentId == studentId && a.CheckInTime >= weekAgo)
            .ToListAsync();

        var totalDays = weeklyRecords.Count;
        var presentDays = weeklyRecords.Count(r => r.Status == AttendanceStatus.Present);
        var lateDays = weeklyRecords.Count(r => r.Status == AttendanceStatus.Late);

        try
        {
            await _emailService.SendWeeklyReportAsync(
                student.Email,
                $"{student.FirstName} {student.LastName}",
                totalDays,
                presentDays,
                lateDays
            );

            return Ok(new { message = "Weekly report sent successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to send email: {ex.Message}" });
        }
    }

    [HttpPost("send-bulk-absent")]
    public async Task<ActionResult> SendBulkAbsentNotifications([FromBody] DateTime date)
    {
        var attendedStudentIds = await _context.AttendanceRecords
            .Where(a => a.CheckInTime.Date == date.Date)
            .Select(a => a.StudentId)
            .ToListAsync();

        var absentStudents = await _context.Students
            .Where(s => !attendedStudentIds.Contains(s.Id))
            .ToListAsync();

        int successCount = 0;
        int failCount = 0;

        foreach (var student in absentStudents)
        {
            try
            {
                await _emailService.SendAbsentNotificationAsync(
                    student.Email,
                    $"{student.FirstName} {student.LastName}",
                    date
                );
                successCount++;
            }
            catch
            {
                failCount++;
            }
        }

        return Ok(new
        {
            message = $"Sent {successCount} notifications, {failCount} failed",
            totalAbsent = absentStudents.Count,
            successCount,
            failCount
        });
    }
}

public class AbsentNotificationRequest
{
    public int StudentId { get; set; }
    public DateTime Date { get; set; }
}
