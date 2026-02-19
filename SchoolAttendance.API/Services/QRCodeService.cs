using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace SchoolAttendance.API.Services;

public interface IQRCodeService
{
    byte[] GenerateQRCode(string text);
    string GenerateQRCodeBase64(string text);
}

public class QRCodeService : IQRCodeService
{
    public byte[] GenerateQRCode(string text)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCode(qrCodeData);
        using var bitmap = qrCode.GetGraphic(20);
        
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    public string GenerateQRCodeBase64(string text)
    {
        var qrBytes = GenerateQRCode(text);
        return Convert.ToBase64String(qrBytes);
    }
}