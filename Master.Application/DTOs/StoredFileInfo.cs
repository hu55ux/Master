namespace Master.Application.DTOs;

public class StoredFileInfo
{
    public string StorageKey { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public long Size { get; set; }
}
