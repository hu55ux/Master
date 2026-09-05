namespace Master.Application.DTOs;

/// <summary>
/// DTO response after uploading media or file to Cloudinary for Chat sharing.
/// </summary>
public class UploadMediaResponseDTO
{
    /// <summary>
    /// The secure HTTPS Cloudinary CDN URL of the uploaded image/file.
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// The original file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// High-level media category ("image", "document", "audio", "video").
    /// </summary>
    public string MediaType { get; set; } = "image";
}
