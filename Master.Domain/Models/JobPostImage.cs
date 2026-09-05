namespace Master.Domain.Models;

/// <summary>
/// Represents an image attachment for a Job Post demonstrating the job requirements or site details.
/// </summary>
public class JobPostImage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobPostId { get; set; }
    public virtual JobPost? JobPost { get; set; }

    /// <summary>
    /// The Cloudinary CDN URL of the uploaded job photo.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
}
