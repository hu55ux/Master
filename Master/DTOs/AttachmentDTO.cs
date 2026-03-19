namespace Master.DTOs;

public class UserAttachmentResponseDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}