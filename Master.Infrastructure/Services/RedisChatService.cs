using Master.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Master.Infrastructure.Services;

/// <summary>
/// Redis-backed state manager for online users, connections, and typing status.
/// </summary>
public class RedisChatService : IRedisChatService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisChatService> _logger;

    public RedisChatService(IDistributedCache cache, ILogger<RedisChatService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    private static string GetOnlineKey(Guid userId) => $"chat:online:{userId}";
    private static string GetConnectionsKey(Guid userId) => $"chat:connections:{userId}";
    private static string GetTypingKey(Guid roomId, Guid userId) => $"chat:typing:{roomId}:{userId}";

    public async Task AddUserConnectionAsync(Guid userId, string connectionId)
    {
        try
        {
            var connectionsKey = GetConnectionsKey(userId);
            var existingJson = await _cache.GetStringAsync(connectionsKey);
            var connections = string.IsNullOrEmpty(existingJson) 
                ? new List<string>() 
                : JsonSerializer.Deserialize<List<string>>(existingJson) ?? new List<string>();

            if (!connections.Contains(connectionId))
            {
                connections.Add(connectionId);
            }

            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(24)
            };

            await _cache.SetStringAsync(connectionsKey, JsonSerializer.Serialize(connections), options);
            await _cache.SetStringAsync(GetOnlineKey(userId), "true", options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user connection for UserId: {UserId}", userId);
        }
    }

    public async Task<bool> RemoveUserConnectionAsync(Guid userId, string connectionId)
    {
        try
        {
            var connectionsKey = GetConnectionsKey(userId);
            var existingJson = await _cache.GetStringAsync(connectionsKey);
            if (string.IsNullOrEmpty(existingJson))
            {
                await _cache.RemoveAsync(GetOnlineKey(userId));
                return true;
            }

            var connections = JsonSerializer.Deserialize<List<string>>(existingJson) ?? new List<string>();
            connections.Remove(connectionId);

            if (connections.Count > 0)
            {
                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(24)
                };
                await _cache.SetStringAsync(connectionsKey, JsonSerializer.Serialize(connections), options);
                return false; // Still has active connections
            }

            // No active connections remain
            await _cache.RemoveAsync(connectionsKey);
            await _cache.RemoveAsync(GetOnlineKey(userId));
            return true; // Now offline
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user connection for UserId: {UserId}", userId);
            return true;
        }
    }

    public async Task<bool> IsUserOnlineAsync(Guid userId)
    {
        try
        {
            var isOnline = await _cache.GetStringAsync(GetOnlineKey(userId));
            return isOnline == "true";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user online status for UserId: {UserId}", userId);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetUserConnectionsAsync(Guid userId)
    {
        try
        {
            var existingJson = await _cache.GetStringAsync(GetConnectionsKey(userId));
            if (string.IsNullOrEmpty(existingJson)) return Enumerable.Empty<string>();

            return JsonSerializer.Deserialize<List<string>>(existingJson) ?? Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user connections for UserId: {UserId}", userId);
            return Enumerable.Empty<string>();
        }
    }

    public async Task SetTypingStatusAsync(Guid roomId, Guid userId, bool isTyping)
    {
        try
        {
            var key = GetTypingKey(roomId, userId);
            if (isTyping)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5) // Auto expire typing indicator after 5s
                };
                await _cache.SetStringAsync(key, "true", options);
            }
            else
            {
                await _cache.RemoveAsync(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting typing status for RoomId: {RoomId}, UserId: {UserId}", roomId, userId);
        }
    }

    public async Task<bool> GetTypingStatusAsync(Guid roomId, Guid userId)
    {
        try
        {
            var status = await _cache.GetStringAsync(GetTypingKey(roomId, userId));
            return status == "true";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting typing status for RoomId: {RoomId}, UserId: {UserId}", roomId, userId);
            return false;
        }
    }
}
