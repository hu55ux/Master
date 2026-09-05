namespace Master.Domain.Models;

/// <summary>
/// Represents a dialogue/chat room between a Customer and a Seller (Master).
/// </summary>
public class ChatRoom
{
    /// <summary>
    /// Unique identifier of the chat room.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the customer participating in the chat.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Navigation property for the customer user.
    /// </summary>
    public AppUser? Customer { get; set; }

    /// <summary>
    /// Identifier of the seller (master/service provider) participating in the chat.
    /// </summary>
    public Guid SellerId { get; set; }

    /// <summary>
    /// Navigation property for the seller user.
    /// </summary>
    public AppUser? Seller { get; set; }

    /// <summary>
    /// Optional context product ID associated with this dialogue.
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>
    /// Date and time when the chat room was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Date and time when the chat room was last active/updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Collection of messages sent within this chat room.
    /// </summary>
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
