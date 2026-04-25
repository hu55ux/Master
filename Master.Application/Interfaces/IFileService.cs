using Microsoft.AspNetCore.Http;

namespace Master.Application.Interfaces;

/// <summary>
/// Interface for file management services (S3, Azure Blob, etc.).
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Uploads a file to the storage provider.
    /// </summary>
    Task<string> UploadFileAsync(IFormFile file, string folderName = "uploads");

    /// <summary>
    /// Deletes a file from the storage provider.
    /// </summary>
    Task DeleteFileAsync(string fileUrl);

    /// <summary>
    /// Lists all files in the storage provider, optionally filtered by prefix.
    /// </summary>
    Task<IEnumerable<FileMetadata>> ListFilesAsync(string? prefix = null);
}

/// <summary>
/// Represents metadata about a stored file.
/// </summary>
public class FileMetadata
{
    public string Key { get; set; } = null!;
    public string Url { get; set; } = null!;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
}
