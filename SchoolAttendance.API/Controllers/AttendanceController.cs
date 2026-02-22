using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolAttendance.API.Data;
using SchoolAttendance.API.Hubs;
using SchoolAttendance.API.Models;

namespace SchoolAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<AttendanceHub> _hubContext;

    public AttendanceController(AppDbContext context, IHubContext<AttendanceHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // POST: api/attendance/checkin
    [HttpPost("checkin")]
    public async Task<ActionResult<AttendanceRecord>> CheckIn([FromBody] CheckInRequest request)
    {
        // Find student by QR code
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.QRCode == request.QRCode);

        if (student == null)
        {
            return NotFound(new { message = "Invalid QR code" });
        }

        // Check if already checked in today
        var today = DateTime.UtcNow.Date;
        var existingRecord = await _context.AttendanceRecords
            .Where(a => a.StudentId == student.Id && 
                        a.CheckInTime.Date == today &&
                        a.CheckOutTime == null)
            .FirstOrDefaultAsync();

        if (existingRecord != null)
        {
            return BadRequest(new { message = "Student already checked in" });
        }

        // Create attendance record
        var attendance = new AttendanceRecord
        {
            StudentId = student.Id,
            CheckInTime = DateTime.UtcNow,
            Location = request.Location,
            Status = DetermineStatus(DateTime.UtcNow.TimeOfDay)
        };

        _context.AttendanceRecords.Add(attendance);
        await _context.SaveChangesAsync();

        // Load student info for response
        await _context.Entry(attendance).Reference(a => a.Student).LoadAsync();

        await _hubContext.Clients.All.SendAsync("StudentCheckedIn", new
        {
            studentName = $"{student.FirstName} {student.LastName}",
            studentNumber = student.StudentNumber,
            checkInTime = attendance.CheckInTime,
            location = attendance.Location,
            status = attendance.Status.ToString()
        });

        return Ok(new
        {
            message = "Check-in successful",
            attendance = attendance,
            studentName = $"{student.FirstName} {student.LastName}"
        });
    }

    // POST: api/attendance/checkout
    [HttpPost("checkout")]
    public async Task<ActionResult> CheckOut([FromBody] CheckOutRequest request)
    {
        // Find student by QR code
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.QRCode == request.QRCode);

        if (student == null)
        {
            return NotFound(new { message = "Invalid QR code" });
        }

        // Find today's active attendance record
        var today = DateTime.UtcNow.Date;
        var attendance = await _context.AttendanceRecords
            .Where(a => a.StudentId == student.Id && 
                        a.CheckInTime.Date == today &&
                        a.CheckOutTime == null)
            .FirstOrDefaultAsync();

        if (attendance == null)
        {
            return BadRequest(new { message = "No active check-in found" });
        }

        // Update check-out time
        attendance.CheckOutTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var duration = (attendance.CheckOutTime.Value - attendance.CheckInTime).TotalMinutes;

        await _hubContext.Clients.All.SendAsync("StudentCheckedOut", new
        {
            studentName = $"{student.FirstName} {student.LastName}",
            studentNumber = student.StudentNumber,
            checkOutTime = attendance.CheckOutTime,
            durationMinutes = Math.Round(duration, 2)
        });

        return Ok(new
        {
            message = "Check-out successful",
            checkInTime = attendance.CheckInTime,
            checkOutTime = attendance.CheckOutTime,
            durationMinutes = Math.Round(duration, 2),
            studentName = $"{student.FirstName} {student.LastName}"
        });
    }

    // GET: api/attendance/student/{studentId}
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<AttendanceRecord>>> GetStudentAttendance(int studentId)
    {
        var records = await _context.AttendanceRecords
            .Where(a => a.StudentId == studentId)
            .Include(a => a.Student)
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync();

        return Ok(records);
    }

    // GET: api/attendance/today
    [HttpGet("today")]
    public async Task<ActionResult> GetTodayAttendance()
    {
        var today = DateTime.UtcNow.Date;
        var records = await _context.AttendanceRecords
            .Where(a => a.CheckInTime.Date == today)
            .Include(a => a.Student)
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync();

        return Ok(new
        {
            date = today,
            totalRecords = records.Count,
            checkedIn = records.Count(r => r.CheckOutTime == null),
            checkedOut = records.Count(r => r.CheckOutTime != null),
            records = records
        });
    }

    // GET: api/attendance/date/{date}
    [HttpGet("date/{date}")]
    public async Task<ActionResult> GetAttendanceByDate(DateTime date)
    {
        var records = await _context.AttendanceRecords
            .Where(a => a.CheckInTime.Date == date.Date)
            .Include(a => a.Student)
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync();

        return Ok(new
        {
            date = date.Date,
            totalRecords = records.Count,
            records = records
        });
    }

    private AttendanceStatus DetermineStatus(TimeSpan checkInTime)
    {
        // Assume class starts at 9:00 AM
        var classStartTime = new TimeSpan(9, 0, 0);
        var lateThreshold = new TimeSpan(9, 15, 0); // 15 minutes late

        if (checkInTime <= classStartTime)
        {
            return AttendanceStatus.Present;
        }
        else if (checkInTime <= lateThreshold)
        {
            return AttendanceStatus.Late;
        }
        else
        {
            return AttendanceStatus.Late;
        }
    }
}

// Request DTOs
public class CheckInRequest
{
    public string QRCode { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public class CheckOutRequest
{
    public string QRCode { get; set; } = string.Empty;
}