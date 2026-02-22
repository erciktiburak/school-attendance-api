using MailKit.Net.Smtp;
using MimeKit;
using SchoolAttendance.API.Models;

namespace SchoolAttendance.API.Services;

public interface IEmailService
{
    Task SendAbsentNotificationAsync(string studentEmail, string studentName, DateTime date);
    Task SendLateNotificationAsync(string studentEmail, string studentName, DateTime checkInTime);
    Task SendWeeklyReportAsync(string recipientEmail, string recipientName, int totalDays, int presentDays, int lateDays);
    Task SendCustomEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
        _logger = logger;
    }

    public async Task SendAbsentNotificationAsync(string studentEmail, string studentName, DateTime date)
    {
        var subject = "Attendance Alert: Absence Recorded";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #dc2626;'>Absence Notification</h2>
                    <p>Dear {studentName},</p>
                    <p>This is to inform you that you were marked absent on <strong>{date:MMMM dd, yyyy}</strong>.</p>
                    <p>If you believe this is an error, please contact your teacher or the administration office.</p>
                    <hr style='margin: 20px 0;'>
                    <p style='color: #6b7280; font-size: 12px;'>
                        This is an automated message from the School Attendance System.
                    </p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(studentEmail, subject, body);
    }

    public async Task SendLateNotificationAsync(string studentEmail, string studentName, DateTime checkInTime)
    {
        var subject = "Attendance Alert: Late Arrival";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #f59e0b;'>Late Arrival Notification</h2>
                    <p>Dear {studentName},</p>
                    <p>You checked in late on <strong>{checkInTime:MMMM dd, yyyy}</strong> at <strong>{checkInTime:HH:mm}</strong>.</p>
                    <p>Please ensure you arrive on time for future sessions.</p>
                    <hr style='margin: 20px 0;'>
                    <p style='color: #6b7280; font-size: 12px;'>
                        This is an automated message from the School Attendance System.
                    </p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(studentEmail, subject, body);
    }

    public async Task SendWeeklyReportAsync(string recipientEmail, string recipientName, int totalDays, int presentDays, int lateDays)
    {
        var attendanceRate = totalDays > 0 ? Math.Round((double)presentDays / totalDays * 100, 2) : 0;

        var subject = "Weekly Attendance Report";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4f46e5;'>Weekly Attendance Summary</h2>
                    <p>Dear {recipientName},</p>
                    <p>Here is your attendance summary for this week:</p>

                    <div style='background: #f3f4f6; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                        <table style='width: 100%;'>
                            <tr>
                                <td style='padding: 10px;'><strong>Total Days:</strong></td>
                                <td style='text-align: right;'>{totalDays}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px;'><strong>Present:</strong></td>
                                <td style='text-align: right; color: #10b981;'>{presentDays}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px;'><strong>Late:</strong></td>
                                <td style='text-align: right; color: #f59e0b;'>{lateDays}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px;'><strong>Attendance Rate:</strong></td>
                                <td style='text-align: right; font-size: 18px; font-weight: bold; color: #4f46e5;'>{attendanceRate}%</td>
                            </tr>
                        </table>
                    </div>

                    <p>Keep up the good work!</p>
                    <hr style='margin: 20px 0;'>
                    <p style='color: #6b7280; font-size: 12px;'>
                        This is an automated weekly report from the School Attendance System.
                    </p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendCustomEmailAsync(string to, string subject, string body)
    {
        await SendEmailAsync(to, subject, body);
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_emailSettings.SmtpServer) || string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
        {
            _logger.LogWarning("Email not configured; skipping send to {To}", to);
            return;
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.EnableSsl);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }
}
