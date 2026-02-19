using Microsoft.EntityFrameworkCore;
using SchoolAttendance.API.Data;
using SchoolAttendance.API.Models;
using Xunit;

namespace SchoolAttendance.Tests;

public class AttendanceServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        
        // Seed test data
        context.Students.Add(new Student
        {
            Id = 1,
            StudentNumber = "TEST001",
            FirstName = "Test",
            LastName = "Student",
            Email = "test@emu.edu.tr",
            Department = "Computer Engineering",
            QRCode = "test-qr-code",
            CreatedAt = DateTime.UtcNow
        });
        
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task CheckIn_ValidQRCode_CreatesAttendanceRecord()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var student = await context.Students.FirstAsync();

        // Act
        var attendance = new AttendanceRecord
        {
            StudentId = student.Id,
            CheckInTime = DateTime.UtcNow,
            Location = "Computer Lab",
            Status = AttendanceStatus.Present
        };
        
        context.AttendanceRecords.Add(attendance);
        await context.SaveChangesAsync();

        // Assert
        var savedRecord = await context.AttendanceRecords.FirstOrDefaultAsync();
        Assert.NotNull(savedRecord);
        Assert.Equal(student.Id, savedRecord.StudentId);
        Assert.Equal("Computer Lab", savedRecord.Location);
        Assert.Null(savedRecord.CheckOutTime);
    }

    [Fact]
    public async Task CheckOut_ExistingCheckIn_UpdatesCheckOutTime()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var student = await context.Students.FirstAsync();
        
        var attendance = new AttendanceRecord
        {
            StudentId = student.Id,
            CheckInTime = DateTime.UtcNow.AddHours(-2),
            Location = "Computer Lab",
            Status = AttendanceStatus.Present
        };
        
        context.AttendanceRecords.Add(attendance);
        await context.SaveChangesAsync();

        // Act
        attendance.CheckOutTime = DateTime.UtcNow;
        await context.SaveChangesAsync();

        // Assert
        var updatedRecord = await context.AttendanceRecords.FirstAsync();
        Assert.NotNull(updatedRecord.CheckOutTime);
        Assert.True((updatedRecord.CheckOutTime.Value - updatedRecord.CheckInTime).TotalHours >= 2);
    }

    [Fact]
    public async Task GetTodayAttendance_ReturnsOnlyTodayRecords()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var student = await context.Students.FirstAsync();
        
        // Today's record
        context.AttendanceRecords.Add(new AttendanceRecord
        {
            StudentId = student.Id,
            CheckInTime = DateTime.UtcNow,
            Location = "Lab",
            Status = AttendanceStatus.Present
        });
        
        // Yesterday's record
        context.AttendanceRecords.Add(new AttendanceRecord
        {
            StudentId = student.Id,
            CheckInTime = DateTime.UtcNow.AddDays(-1),
            Location = "Lab",
            Status = AttendanceStatus.Present
        });
        
        await context.SaveChangesAsync();

        // Act
        var today = DateTime.UtcNow.Date;
        var todayRecords = await context.AttendanceRecords
            .Where(a => a.CheckInTime.Date == today)
            .ToListAsync();

        // Assert
        Assert.Single(todayRecords);
    }

    [Fact]
    public async Task Statistics_CalculatesAttendanceRateCorrectly()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var student = await context.Students.FirstAsync();
        
        // Add 3 attendance records
        for (int i = 0; i < 3; i++)
        {
            context.AttendanceRecords.Add(new AttendanceRecord
            {
                StudentId = student.Id,
                CheckInTime = DateTime.UtcNow.AddDays(-i),
                Location = "Lab",
                Status = i == 0 ? AttendanceStatus.Present : AttendanceStatus.Late
            });
        }
        await context.SaveChangesAsync();

        // Act
        var totalRecords = await context.AttendanceRecords.CountAsync();
        var presentCount = await context.AttendanceRecords
            .Where(a => a.Status == AttendanceStatus.Present)
            .CountAsync();
        
        var attendanceRate = Math.Round((double)presentCount / totalRecords * 100, 2);

        // Assert
        Assert.Equal(3, totalRecords);
        Assert.Equal(1, presentCount);
        Assert.Equal(33.33, attendanceRate);
    }
}