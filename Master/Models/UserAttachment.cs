namespace Master.Models;

public class UserAttachment
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public Guid UploadedUserId { get; set; }
    public AppUser UploadedUser { get; set; } = null!;
    public DateTimeOffset UploadedAt { get; set; }
}