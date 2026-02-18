using Microsoft.EntityFrameworkCore;
using SchoolAttendance.API.Models;

namespace SchoolAttendance.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.StudentNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.QRCode).IsUnique();
            entity.Property(e => e.StudentNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        });

        // Teacher configuration
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        });

        // AttendanceRecord configuration
        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.AttendanceRecords)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.StudentId, e.CheckInTime });
        });

        // Seed data
        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                Id = 1,
                StudentNumber = "20230001",
                FirstName = "Ahmet",
                LastName = "Yılmaz",
                Email = "ahmet.yilmaz@emu.edu.tr",
                Department = "Computer Engineering",
                QRCode = "ahmet-qr-fixed-code", 
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) 
            },
            new Student
            {
                Id = 2,
                StudentNumber = "20230002",
                FirstName = "Ayşe",
                LastName = "Demir",
                Email = "ayse.demir@emu.edu.tr",
                Department = "Computer Engineering",
                QRCode = "ayse-qr-fixed-code", 
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Teacher>().HasData(
            new Teacher
            {
                Id = 1,
                EmployeeNumber = "T001",
                FirstName = "Dr. Mehmet",
                LastName = "Kaya",
                Email = "mehmet.kaya@emu.edu.tr",
                Department = "Computer Engineering",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}