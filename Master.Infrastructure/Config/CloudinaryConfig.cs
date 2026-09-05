namespace Master.Infrastructure.Config;

/// <summary>
/// Configuration settings for Cloudinary cloud image & media storage service.
/// </summary>
public class CloudinaryConfig
{
    public const string SectionName = "Cloudinary";

    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}
