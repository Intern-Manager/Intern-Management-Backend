using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace InternManagement.API.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _cloudName;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"]
            ?? throw new InvalidOperationException("Cloudinary:CloudName is not configured");
        var apiKey = configuration["Cloudinary:ApiKey"]
            ?? throw new InvalidOperationException("Cloudinary:ApiKey is not configured");
        var apiSecret = configuration["Cloudinary:ApiSecret"]
            ?? throw new InvalidOperationException("Cloudinary:ApiSecret is not configured");

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudName = cloudName;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default)
    {
        fileStream.Position = 0;

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        string url;

        if (ext is ".png" or ".jpg" or ".jpeg" or ".gif" or ".webp" or ".svg")
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = $"intern-management/{folder}",
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false,
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.Error != null)
                throw new InvalidOperationException($"Cloudinary upload error: {result.Error.Message}");
            url = result.SecureUrl.ToString();
        }
        else if (ext is ".mp4" or ".mov" or ".avi" or ".webm")
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = $"intern-management/{folder}",
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false,
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.Error != null)
                throw new InvalidOperationException($"Cloudinary upload error: {result.Error.Message}");
            url = result.SecureUrl.ToString();
        }
        else
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = $"intern-management/{folder}",
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false,
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.Error != null)
                throw new InvalidOperationException($"Cloudinary upload error: {result.Error.Message}");
            url = result.SecureUrl.ToString();
        }

        return url;
    }

    public async Task<(byte[] Data, string ContentType, string FileName)?> DownloadFileAsync(string publicUrl, CancellationToken ct = default)
    {
        var publicId = ExtractPublicId(publicUrl);
        if (string.IsNullOrEmpty(publicId))
            return null;

        var uri = new Uri(publicUrl);
        var path = uri.AbsolutePath;
        var fileName = Path.GetFileName(path);
        var ext = Path.GetExtension(path).ToLowerInvariant();
        
        var contentType = ext switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        try
        {
            var downloadUrl = _cloudinary.DownloadPrivate(publicId, expiresAt: DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds());
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(downloadUrl, ct);
            return (bytes, contentType, fileName);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteFileAsync(string publicUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(publicUrl)) return true;

        var publicId = ExtractPublicId(publicUrl);
        if (string.IsNullOrEmpty(publicId)) return true;

        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);
        return result.Error == null;
    }

    private string ExtractPublicId(string url)
    {
        try
        {
            var uri = new Uri(url);
            var path = uri.AbsolutePath;
            var marker = "/upload/";
            var idx = path.IndexOf(marker, StringComparison.Ordinal);
            if (idx >= 0)
            {
                var withoutPrefix = path[(idx + marker.Length)..];
                return Path.GetFileNameWithoutExtension(withoutPrefix);
            }
            return Path.GetFileNameWithoutExtension(path);
        }
        catch
        {
            return string.Empty;
        }
    }
}
