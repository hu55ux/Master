namespace Master.Application.Interfaces;

/// <summary>
/// Service interface for managing real-time chat state in Redis (User online status, connection tracking, typing indicators).
/// </summary>
public interface IRedisChatService
{
    /// <summary>
    /// Tracks user connection and marks user as Online in Redis.
    /// </summary>
    Task AddUserConnectionAsync(Guid userId, string connectionId);

    /// <summary>
    /// Removes user connection and marks user Offline if no active connections remain.
    /// </summary>
    Task<bool> RemoveUserConnectionAsync(Guid userId, string connectionId);

    /// <summary>
    /// Checks if a user is currently online.
    /// </summary>
    Task<bool> IsUserOnlineAsync(Guid userId);

    /// <summary>
    /// Gets all active connection IDs for a user.
    /// </summary>
    Task<IEnumerable<string>> GetUserConnectionsAsync(Guid userId);

    /// <summary>
    /// Sets user typing status for a chat room with temporary TTL.
    /// </summary>
    Task SetTypingStatusAsync(Guid roomId, Guid userId, bool isTyping);

    /// <summary>
    /// Gets typing status of a user in a chat room.
    /// </summary>
    Task<bool> GetTypingStatusAsync(Guid roomId, Guid userId);
}
