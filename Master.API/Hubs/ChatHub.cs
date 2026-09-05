using System.Security.Claims;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Enums;
using Master.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Master.API.Hubs;

/// <summary>
/// High-performance SignalR Hub for real-time e-commerce messaging, product card sharing, online presence, typing status, and mobile push notifications.
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly IChatRepository _chatRepository;
    private readonly IRedisChatService _redisChatService;
    private readonly IPushNotificationService _pushNotificationService;

    public ChatHub(
        IChatRepository chatRepository, 
        IRedisChatService redisChatService,
        IPushNotificationService pushNotificationService)
    {
        _chatRepository = chatRepository;
        _redisChatService = redisChatService;
        _pushNotificationService = pushNotificationService;
    }

    private Guid UserId => Guid.Parse(Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) 
        ?? throw new HubException("User is not authenticated."));

    private string UserName => $"{Context.User?.FindFirstValue(ClaimTypes.GivenName)} {Context.User?.FindFirstValue(ClaimTypes.Surname)}".Trim();

    public override async Task OnConnectedAsync()
    {
        var userId = UserId;
        await _redisChatService.AddUserConnectionAsync(userId, Context.ConnectionId);

        // Notify other clients about user online status
        await Clients.Others.SendAsync("UserStatusChanged", new { UserId = userId, IsOnline = true });

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = UserId;
        bool isOfflineNow = await _redisChatService.RemoveUserConnectionAsync(userId, Context.ConnectionId);

        if (isOfflineNow)
        {
            await Clients.Others.SendAsync("UserStatusChanged", new { UserId = userId, IsOnline = false });
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a specific chat room group to receive live messages.
    /// </summary>
    public async Task JoinRoom(Guid roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    /// <summary>
    /// Leave a chat room group.
    /// </summary>
    public async Task LeaveRoom(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
    }

    /// <summary>
    /// Sends a standard text message to a chat room.
    /// If recipient is offline or not actively in room, dispatches FCM Mobile Push Notification.
    /// </summary>
    public async Task SendMessageToRoom(Guid roomId, string messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText)) return;

        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ChatRoomId = roomId,
            SenderId = UserId,
            MessageText = messageText,
            Type = MessageType.Text,
            SentAt = DateTimeOffset.UtcNow,
            IsRead = false
        };

        var savedMessage = await _chatRepository.SaveMessageAsync(message, Context.ConnectionAborted);

        var dto = new ChatMessageResponseDTO
        {
            Id = savedMessage.Id,
            ChatRoomId = savedMessage.ChatRoomId,
            SenderId = savedMessage.SenderId,
            SenderName = savedMessage.Sender != null ? $"{savedMessage.Sender.FirstName} {savedMessage.Sender.LastName}".Trim() : UserName,
            MessageText = savedMessage.MessageText,
            Type = savedMessage.Type.ToString(),
            SentAt = savedMessage.SentAt,
            IsRead = savedMessage.IsRead
        };

        // Broadcast to all active users in the room
        await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", dto);

        // Check if recipient is offline to dispatch FCM Push Notification to Mobile App
        var room = await _chatRepository.GetChatRoomByIdAsync(roomId, Context.ConnectionAborted);
        if (room != null)
        {
            var recipientId = room.CustomerId == UserId ? room.SellerId : room.CustomerId;
            bool isRecipientOnline = await _redisChatService.IsUserOnlineAsync(recipientId);

            if (!isRecipientOnline)
            {
                await _pushNotificationService.SendPushNotificationAsync(
                    recipientId,
                    title: dto.SenderName,
                    body: dto.MessageText,
                    data: new Dictionary<string, string>
                    {
                        { "type", "chat_message" },
                        { "roomId", roomId.ToString() },
                        { "senderId", UserId.ToString() }
                    },
                    cancellationToken: Context.ConnectionAborted);
            }
        }
    }

    /// <summary>
    /// Sends an interactive Product Card context message within the chat room.
    /// </summary>
    public async Task SendProductCard(Guid roomId, Guid productId, string productName, decimal productPrice, string? imageUrl)
    {
        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ChatRoomId = roomId,
            SenderId = UserId,
            MessageText = $"Product Shared: {productName}",
            Type = MessageType.ProductCard,
            ProductId = productId,
            ProductName = productName,
            ProductPrice = productPrice,
            ProductImageUrl = imageUrl,
            SentAt = DateTimeOffset.UtcNow,
            IsRead = false
        };

        var savedMessage = await _chatRepository.SaveMessageAsync(message, Context.ConnectionAborted);

        var dto = new ChatMessageResponseDTO
        {
            Id = savedMessage.Id,
            ChatRoomId = savedMessage.ChatRoomId,
            SenderId = savedMessage.SenderId,
            SenderName = savedMessage.Sender != null ? $"{savedMessage.Sender.FirstName} {savedMessage.Sender.LastName}".Trim() : UserName,
            MessageText = savedMessage.MessageText,
            Type = savedMessage.Type.ToString(),
            ProductId = savedMessage.ProductId,
            ProductName = savedMessage.ProductName,
            ProductPrice = savedMessage.ProductPrice,
            ProductImageUrl = savedMessage.ProductImageUrl,
            SentAt = savedMessage.SentAt,
            IsRead = savedMessage.IsRead
        };

        await Clients.Group(roomId.ToString()).SendAsync("ReceiveProductCard", dto);
        await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", dto);

        var room = await _chatRepository.GetChatRoomByIdAsync(roomId, Context.ConnectionAborted);
        if (room != null)
        {
            var recipientId = room.CustomerId == UserId ? room.SellerId : room.CustomerId;
            bool isRecipientOnline = await _redisChatService.IsUserOnlineAsync(recipientId);

            if (!isRecipientOnline)
            {
                await _pushNotificationService.SendPushNotificationAsync(
                    recipientId,
                    title: dto.SenderName,
                    body: $"Shared product: {productName}",
                    data: new Dictionary<string, string>
                    {
                        { "type", "product_card" },
                        { "roomId", roomId.ToString() },
                        { "productId", productId.ToString() }
                    },
                    cancellationToken: Context.ConnectionAborted);
            }
        }
    }

    /// <summary>
    /// Broadcasts "typing..." status indicator in real-time.
    /// </summary>
    public async Task SendTypingStatus(Guid roomId, bool isTyping)
    {
        await _redisChatService.SetTypingStatusAsync(roomId, UserId, isTyping);
        await Clients.Group(roomId.ToString()).SendAsync("UserTyping", new { RoomId = roomId, UserId = UserId, IsTyping = isTyping });
    }

    /// <summary>
    /// Marks messages as read in real-time.
    /// </summary>
    public async Task MarkMessagesAsRead(Guid roomId)
    {
        var readCount = await _chatRepository.MarkMessagesAsReadAsync(roomId, UserId, Context.ConnectionAborted);
        if (readCount > 0)
        {
            await Clients.Group(roomId.ToString()).SendAsync("MessagesRead", new { RoomId = roomId, ReadCount = readCount, ReadByUserId = UserId });
        }
    }
}
