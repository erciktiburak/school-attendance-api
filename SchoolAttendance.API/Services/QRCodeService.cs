using QRCoder;

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
        using var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }

    public string GenerateQRCodeBase64(string text)
    {
        var qrBytes = GenerateQRCode(text);
        return Convert.ToBase64String(qrBytes);
    }
}