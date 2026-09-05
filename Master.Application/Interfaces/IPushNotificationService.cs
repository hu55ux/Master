namespace Master.Application.Interfaces;

/// <summary>
/// Service interface for dispatching Push Notifications to Mobile devices (iOS & Android via FCM / Firebase).
/// </summary>
public interface IPushNotificationService
{
    /// <summary>
    /// Sends a push notification to a specific user's registered mobile device.
    /// </summary>
    Task<bool> SendPushNotificationAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a push notification using a direct FCM device token.
    /// </summary>
    Task<bool> SendToDeviceTokenAsync(string deviceToken, string title, string body, IDictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}
