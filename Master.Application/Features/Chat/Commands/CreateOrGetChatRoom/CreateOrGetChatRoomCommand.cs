using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Chat.Commands.CreateOrGetChatRoom;

public record CreateOrGetChatRoomCommand(Guid CurrentUserId, Guid SellerId, Guid? ProductId) : IRequest<ChatRoomResponseDTO>;

public class CreateOrGetChatRoomHandler : IRequestHandler<CreateOrGetChatRoomCommand, ChatRoomResponseDTO>
{
    private readonly IChatRepository _chatRepository;
    private readonly IRedisChatService _redisService;

    public CreateOrGetChatRoomHandler(IChatRepository chatRepository, IRedisChatService redisService)
    {
        _chatRepository = chatRepository;
        _redisService = redisService;
    }

    public async Task<ChatRoomResponseDTO> Handle(CreateOrGetChatRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _chatRepository.CreateOrGetChatRoomAsync(request.CurrentUserId, request.SellerId, request.ProductId, cancellationToken);
        var partnerId = room.CustomerId == request.CurrentUserId ? room.SellerId : room.CustomerId;
        var isPartnerOnline = await _redisService.IsUserOnlineAsync(partnerId);

        var lastMessage = room.Messages.FirstOrDefault();

        return new ChatRoomResponseDTO
        {
            Id = room.Id,
            CustomerId = room.CustomerId,
            CustomerName = room.Customer != null ? $"{room.Customer.FirstName} {room.Customer.LastName}".Trim() : string.Empty,
            SellerId = room.SellerId,
            SellerName = room.Seller != null ? $"{room.Seller.FirstName} {room.Seller.LastName}".Trim() : string.Empty,
            ProductId = room.ProductId,
            IsPartnerOnline = isPartnerOnline,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt,
            LastMessage = lastMessage != null ? new ChatMessageResponseDTO
            {
                Id = lastMessage.Id,
                ChatRoomId = lastMessage.ChatRoomId,
                SenderId = lastMessage.SenderId,
                MessageText = lastMessage.MessageText,
                Type = lastMessage.Type.ToString(),
                ProductId = lastMessage.ProductId,
                ProductName = lastMessage.ProductName,
                ProductPrice = lastMessage.ProductPrice,
                ProductImageUrl = lastMessage.ProductImageUrl,
                SentAt = lastMessage.SentAt,
                IsRead = lastMessage.IsRead
            } : null
        };
    }
}
