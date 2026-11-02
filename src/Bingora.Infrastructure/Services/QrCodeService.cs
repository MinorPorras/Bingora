using Bingora.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace Bingora.Infrastructure.Services;

public class QrCodeService : IQrCodeService
{
    private readonly string _baseUrl;
    private readonly string _wwwRootPath;

    public QrCodeService(IConfiguration configuration)
    {
        _baseUrl = configuration["AppSettings:BaseUrl"] ?? "https://localhost:5001";
        _wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    }

    public async Task<string> GenerateCardQrAsync(string gameCode, string cardCode, Guid gameId)
    {
        var qrContent = $"{_baseUrl}/join?game={gameCode}&card={cardCode}";

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.M);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrBytes = qrCode.GetGraphic(10);

        var gameFolder = Path.Combine(_wwwRootPath, "qrCodes", "games", gameId.ToString());
        Directory.CreateDirectory(gameFolder);

        var fileName = $"{cardCode}.png";
        var filePath = Path.Combine(gameFolder, fileName);
        await File.WriteAllBytesAsync(filePath, qrBytes);

        return $"/qrcodes/games/{gameId}/{fileName}";
    }

    public async Task RegenerateGameQrAsync(Guid gameId, string gameCode, IEnumerable<string> cardCodes)
    {
        foreach (var cardCode in cardCodes)
            await GenerateCardQrAsync(gameCode, cardCode, gameId);
    }
}