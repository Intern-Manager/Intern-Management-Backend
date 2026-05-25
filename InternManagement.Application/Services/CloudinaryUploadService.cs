using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace InternManagement.Application.Services;

public class CloudinaryUploadService : IImageUploadService
{
    private readonly Cloudinary? _cloudinary;
    private readonly string _cloudName;
    private readonly bool _isConfigured;

    public CloudinaryUploadService(IConfiguration configuration)
    {
        try
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (!string.IsNullOrEmpty(cloudName) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(apiSecret))
            {
                var account = new Account(cloudName, apiKey, apiSecret);
                _cloudinary = new Cloudinary(account);
                _cloudName = cloudName;
                _isConfigured = true;
            }
            else
            {
                _isConfigured = false;
                _cloudName = string.Empty;
            }
        }
        catch
        {
            _isConfigured = false;
            _cloudName = string.Empty;
        }
    }

    public async Task<string> UploadAvatarAsync(string base64Image, CancellationToken ct = default)
    {
        if (!_isConfigured || _cloudinary == null)
        {
            throw new InvalidOperationException("Cloudinary is not configured");
        }

        if (string.IsNullOrWhiteSpace(base64Image))
        {
            throw new ArgumentException("Base64 image data is required", nameof(base64Image));
        }

        var base64Data = base64Image.Contains(',')
            ? base64Image.Split(',')[1]
            : base64Image;

        var bytes = Convert.FromBase64String(base64Data);
        
        using var stream = new MemoryStream(bytes);
        var fileName = $"avatar_{Guid.NewGuid()}.png";

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = "intern-management/avatars",
            Transformation = new Transformation().Width(200).Height(200).Crop("fill").Gravity("face"),
            PublicId = fileName.Replace(".png", "")
        };

        var result = await _cloudinary.UploadAsync(uploadParams, ct);
        
        if (result.Error != null)
        {
            throw new Exception($"Cloudinary upload failed: {result.Error.Message}");
        }

        return result.SecureUrl.ToString();
    }
}
