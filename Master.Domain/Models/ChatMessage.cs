using Master.Domain.Enums;

namespace Master.Domain.Models;

/// <summary>
/// Represents an individual message sent in a chat room.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Unique identifier of the message.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the associated chat room.
    /// </summary>
    public Guid ChatRoomId { get; set; }

    /// <summary>
    /// Navigation property for the parent chat room.
    /// </summary>
    public ChatRoom? ChatRoom { get; set; }

    /// <summary>
    /// Identifier of the user who sent the message.
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// Navigation property for the sender user.
    /// </summary>
    public AppUser? Sender { get; set; }

    /// <summary>
    /// Text content of the message.
    /// </summary>
    public string MessageText { get; set; } = string.Empty;

    /// <summary>
    /// Type of the message (Text or ProductCard).
    /// </summary>
    public MessageType Type { get; set; } = MessageType.Text;

    #region Product Card Context (E-Commerce)

    /// <summary>
    /// Associated product ID when sending a ProductCard message.
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>
    /// Product name snapshot at the time of sending the card.
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// Product price snapshot at the time of sending the card.
    /// </summary>
    public decimal? ProductPrice { get; set; }

    /// <summary>
    /// Image URL of the product card.
    /// </summary>
    public string? ProductImageUrl { get; set; }

    #endregion

    /// <summary>
    /// Timestamp when the message was sent.
    /// </summary>
    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Indicates whether the message has been read by the recipient.
    /// </summary>
    public bool IsRead { get; set; } = false;
}
