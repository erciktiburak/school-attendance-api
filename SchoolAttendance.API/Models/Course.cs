namespace SchoolAttendance.API.Models;

public class Course
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Schedule { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
    public ICollection<CourseAttendance> CourseAttendances { get; set; } = new List<CourseAttendance>();
}

public class CourseEnrollment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}

public class CourseAttendance
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public DateTime Date { get; set; }
    public DateTime CheckInTime { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}
