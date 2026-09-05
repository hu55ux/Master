namespace Master.Domain.Enums;

/// <summary>
/// Defines the type of message sent in a chat room.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Standard text message.
    /// </summary>
    Text = 1,

    /// <summary>
    /// Interactive Product Card message for e-commerce context.
    /// </summary>
    ProductCard = 2
}
