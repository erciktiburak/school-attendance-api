using OfficeOpenXml;
using OfficeOpenXml.Style;
using SchoolAttendance.API.Models;
using System.Drawing;

namespace SchoolAttendance.API.Services;

public interface IExcelExportService
{
    Task<byte[]> ExportAttendanceToExcel(IEnumerable<AttendanceRecord> records, DateTime date);
    Task<byte[]> ExportStudentsToExcel(IEnumerable<Student> students);
}

public class ExcelExportService : IExcelExportService
{
    public async Task<byte[]> ExportAttendanceToExcel(IEnumerable<AttendanceRecord> records, DateTime date)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add($"Attendance - {date:yyyy-MM-dd}");
        
        // Header styling
        worksheet.Cells["A1:G1"].Style.Font.Bold = true;
        worksheet.Cells["A1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells["A1:G1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 70, 229));
        worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.White);
        
        // Headers
        worksheet.Cells[1, 1].Value = "Student Number";
        worksheet.Cells[1, 2].Value = "Name";
        worksheet.Cells[1, 3].Value = "Department";
        worksheet.Cells[1, 4].Value = "Check-in Time";
        worksheet.Cells[1, 5].Value = "Check-out Time";
        worksheet.Cells[1, 6].Value = "Location";
        worksheet.Cells[1, 7].Value = "Status";
        
        // Data
        int row = 2;
        foreach (var record in records)
        {
            worksheet.Cells[row, 1].Value = record.Student?.StudentNumber;
            worksheet.Cells[row, 2].Value = $"{record.Student?.FirstName} {record.Student?.LastName}";
            worksheet.Cells[row, 3].Value = record.Student?.Department;
            worksheet.Cells[row, 4].Value = record.CheckInTime.ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[row, 5].Value = record.CheckOutTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "-";
            worksheet.Cells[row, 6].Value = record.Location;
            worksheet.Cells[row, 7].Value = record.Status.ToString();
            
            // Status color coding
            var statusCell = worksheet.Cells[row, 7];
            statusCell.Style.Font.Bold = true;
            switch (record.Status)
            {
                case AttendanceStatus.Present:
                    statusCell.Style.Font.Color.SetColor(Color.Green);
                    break;
                case AttendanceStatus.Late:
                    statusCell.Style.Font.Color.SetColor(Color.Orange);
                    break;
                case AttendanceStatus.Absent:
                    statusCell.Style.Font.Color.SetColor(Color.Red);
                    break;
            }
            
            row++;
        }
        
        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        
        return await Task.FromResult(package.GetAsByteArray());
    }
    
    public async Task<byte[]> ExportStudentsToExcel(IEnumerable<Student> students)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Students");
        
        // Header styling
        worksheet.Cells["A1:E1"].Style.Font.Bold = true;
        worksheet.Cells["A1:E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells["A1:E1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 70, 229));
        worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.White);
        
        // Headers
        worksheet.Cells[1, 1].Value = "Student Number";
        worksheet.Cells[1, 2].Value = "First Name";
        worksheet.Cells[1, 3].Value = "Last Name";
        worksheet.Cells[1, 4].Value = "Email";
        worksheet.Cells[1, 5].Value = "Department";
        
        // Data
        int row = 2;
        foreach (var student in students)
        {
            worksheet.Cells[row, 1].Value = student.StudentNumber;
            worksheet.Cells[row, 2].Value = student.FirstName;
            worksheet.Cells[row, 3].Value = student.LastName;
            worksheet.Cells[row, 4].Value = student.Email;
            worksheet.Cells[row, 5].Value = student.Department;
            row++;
        }
        
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        
        return await Task.FromResult(package.GetAsByteArray());
    }
}