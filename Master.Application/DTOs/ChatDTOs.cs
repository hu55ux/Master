using Master.Domain.Enums;

namespace Master.Application.DTOs;

/// <summary>
/// DTO representing a Chat Room response.
/// </summary>
public class ChatRoomResponseDTO
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public Guid? ProductId { get; set; }

    /// <summary>
    /// Indicates whether the other participant in the chat is online.
    /// </summary>
    public bool IsPartnerOnline { get; set; }

    public int UnreadCount { get; set; }
    public ChatMessageResponseDTO? LastMessage { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// DTO representing a Chat Message response.
/// </summary>
public class ChatMessageResponseDTO
{
    public Guid Id { get; set; }
    public Guid ChatRoomId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public string Type { get; set; } = MessageType.Text.ToString();

    #region Product Card Properties (E-Commerce)
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal? ProductPrice { get; set; }
    public string? ProductImageUrl { get; set; }
    #endregion

    public DateTimeOffset SentAt { get; set; }
    public bool IsRead { get; set; }
}

/// <summary>
/// DTO request for creating or retrieving a Chat Room between customer and seller for a product context.
/// </summary>
public class CreateOrGetChatRoomRequest
{
    public Guid SellerId { get; set; }
    public Guid? ProductId { get; set; }
}

/// <summary>
/// DTO request for sending a standard text message.
/// </summary>
public class SendMessageRequest
{
    public string MessageText { get; set; } = string.Empty;
}

/// <summary>
/// DTO request for sending a product card.
/// </summary>
public class SendProductCardRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public string? ProductImageUrl { get; set; }
}

/// <summary>
/// DTO request for typing status update.
/// </summary>
public class SendTypingStatusRequest
{
    public bool IsTyping { get; set; }
}
