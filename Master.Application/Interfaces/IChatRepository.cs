using Master.Application.Common;
using Master.Domain.Models;

namespace Master.Application.Interfaces;

/// <summary>
/// Repository interface for Chat Room and Chat Message database operations.
/// </summary>
public interface IChatRepository
{
    /// <summary>
    /// Finds or creates a chat room between customer and seller for an optional product.
    /// </summary>
    Task<ChatRoom> CreateOrGetChatRoomAsync(Guid customerId, Guid sellerId, Guid? productId, CancellationToken ct);

    /// <summary>
    /// Gets a chat room by ID with customer and seller details included.
    /// </summary>
    Task<ChatRoom?> GetChatRoomByIdAsync(Guid roomId, CancellationToken ct);

    /// <summary>
    /// Gets all chat rooms involving the specified user.
    /// </summary>
    Task<List<ChatRoom>> GetUserChatRoomsAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Gets paginated message history for a specific chat room ordered by SentAt desc.
    /// </summary>
    Task<PagedResult<ChatMessage>> GetRoomMessagesPagedAsync(Guid roomId, int page, int pageSize, CancellationToken ct);

    /// <summary>
    /// Saves a new message in the database.
    /// </summary>
    Task<ChatMessage> SaveMessageAsync(ChatMessage message, CancellationToken ct);

    /// <summary>
    /// Marks unread messages in a chat room as read for recipient.
    /// </summary>
    Task<int> MarkMessagesAsReadAsync(Guid roomId, Guid currentUserId, CancellationToken ct);

    /// <summary>
    /// Gets total unread messages count for a user across all rooms.
    /// </summary>
    Task<int> GetUnreadCountForUserAsync(Guid userId, CancellationToken ct);
}
