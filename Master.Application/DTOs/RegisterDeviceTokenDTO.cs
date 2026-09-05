namespace Master.Application.DTOs;

/// <summary>
/// Request DTO for registering or updating a mobile user's FCM device token for push notifications.
/// </summary>
public class RegisterDeviceTokenRequest
{
    /// <summary>
    /// FCM Device push token string provided by Firebase SDK on Android/iOS.
    /// </summary>
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Device OS type (e.g. "android", "ios", "web").
    /// </summary>
    public string? DeviceType { get; set; }
}
