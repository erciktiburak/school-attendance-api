namespace SchoolAttendance.API.Models;

public class AttendanceRecord
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Location { get; set; } = string.Empty; // Classroom, Lab, etc.
    public AttendanceStatus Status { get; set; }
    
    // Navigation property
    public Student Student { get; set; } = null!;
}

public enum AttendanceStatus
{
    Present,
    Late,
    Absent,
    Excused
}