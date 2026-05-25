namespace InternManagement.Application.Services;

public interface IImageUploadService
{
    Task<string> UploadAvatarAsync(string base64Image, CancellationToken ct = default);
}
