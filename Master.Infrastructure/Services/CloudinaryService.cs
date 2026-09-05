using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Master.Application.Interfaces;
using Master.Infrastructure.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Master.Infrastructure.Services;

/// <summary>
/// Production Cloudinary media & image upload service implementation.
/// Provides automatic image compression, WebP formatting, global CDN delivery, and secure HTTPS URLs.
/// </summary>
public class CloudinaryService : IFileService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinaryConfig> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret);

        _cloudinary = new Cloudinary(acc);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName = "chat-media")
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File cannot be empty.", nameof(file));

        using var stream = file.OpenReadStream();
        var isImage = file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

        RawUploadResult uploadResult;

        if (isImage)
        {
            var imageParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folderName,
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };
            uploadResult = await _cloudinary.UploadAsync(imageParams);
        }
        else
        {
            var rawParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folderName
            };
            uploadResult = await _cloudinary.UploadAsync(rawParams);
        }

        if (uploadResult.Error != null)
        {
            throw new InvalidOperationException($"Cloudinary upload failed: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl.ToString();
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return;

        var publicId = ExtractPublicIdFromUrl(fileUrl);
        if (!string.IsNullOrEmpty(publicId))
        {
            var deletionParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams);
        }
    }

    public Task<IEnumerable<FileMetadata>> ListFilesAsync(string? prefix = null)
    {
        return Task.FromResult(Enumerable.Empty<FileMetadata>());
    }

    private static string ExtractPublicIdFromUrl(string fileUrl)
    {
        try
        {
            var uri = new Uri(fileUrl);
            var segments = uri.AbsolutePath.Split('/');
            var fileNameWithExt = segments[^1];
            var fileName = Path.GetFileNameWithoutExtension(fileNameWithExt);
            var folder = segments.Length > 2 ? segments[^2] : "";
            return string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";
        }
        catch
        {
            return string.Empty;
        }
    }
}
