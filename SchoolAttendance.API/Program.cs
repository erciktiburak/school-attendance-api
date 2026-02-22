using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using SchoolAttendance.API.Data;
using SchoolAttendance.API.Hubs;
using SchoolAttendance.API.Models;
using SchoolAttendance.API.Services;
using System.Text;
using System.Text.Json.Serialization;

// EPPlus 8+ license (noncommercial)
ExcelPackage.License.SetNonCommercialOrganization("SchoolAttendanceAPI");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IQRCodeService, QRCodeService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGeneration12345";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSignalR();

// CORS (SignalR needs AllowCredentials; use specific origins)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://127.0.0.1:5173", "http://127.0.0.1:5174")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AttendanceHub>("/attendanceHub");

// Auto-migrate database and seed users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    db.Database.Migrate();

    if (!await db.Users.AnyAsync())
    {
        db.Users.Add(new User
        {
            Username = "admin",
            Email = "admin@emu.edu.tr",
            PasswordHash = authService.HashPassword("admin123"),
            Role = UserRole.Admin,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
        db.Users.Add(new User
        {
            Username = "teacher1",
            Email = "teacher@emu.edu.tr",
            PasswordHash = authService.HashPassword("teacher123"),
            Role = UserRole.Teacher,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
        await db.SaveChangesAsync();
    }
}

app.Run();