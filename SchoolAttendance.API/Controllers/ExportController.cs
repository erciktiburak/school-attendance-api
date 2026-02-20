using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAttendance.API.Data;
using SchoolAttendance.API.Services;

namespace SchoolAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IExcelExportService _excelService;

    public ExportController(AppDbContext context, IExcelExportService excelService)
    {
        _context = context;
        _excelService = excelService;
    }

    // GET: api/export/attendance?date=2024-02-20
    [HttpGet("attendance")]
    public async Task<IActionResult> ExportAttendance([FromQuery] DateTime? date)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;
        
        var records = await _context.AttendanceRecords
            .Include(a => a.Student)
            .Where(a => a.CheckInTime.Date == targetDate)
            .OrderBy(a => a.CheckInTime)
            .ToListAsync();

        var excelBytes = await _excelService.ExportAttendanceToExcel(records, targetDate);
        
        var fileName = $"Attendance_{targetDate:yyyy-MM-dd}.xlsx";
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    // GET: api/export/students
    [HttpGet("students")]
    public async Task<IActionResult> ExportStudents()
    {
        var students = await _context.Students
            .OrderBy(s => s.StudentNumber)
            .ToListAsync();

        var excelBytes = await _excelService.ExportStudentsToExcel(students);
        
        var fileName = $"Students_{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}