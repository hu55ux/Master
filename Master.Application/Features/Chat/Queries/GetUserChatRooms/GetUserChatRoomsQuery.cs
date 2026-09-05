using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Chat.Queries.GetUserChatRooms;

public record GetUserChatRoomsQuery(Guid UserId) : IRequest<List<ChatRoomResponseDTO>>;

public class GetUserChatRoomsHandler : IRequestHandler<GetUserChatRoomsQuery, List<ChatRoomResponseDTO>>
{
    private readonly IChatRepository _chatRepository;
    private readonly IRedisChatService _redisService;

    public GetUserChatRoomsHandler(IChatRepository chatRepository, IRedisChatService redisService)
    {
        _chatRepository = chatRepository;
        _redisService = redisService;
    }

    public async Task<List<ChatRoomResponseDTO>> Handle(GetUserChatRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _chatRepository.GetUserChatRoomsAsync(request.UserId, cancellationToken);
        var dtos = new List<ChatRoomResponseDTO>();

        foreach (var room in rooms)
        {
            var partnerId = room.CustomerId == request.UserId ? room.SellerId : room.CustomerId;
            var isPartnerOnline = await _redisService.IsUserOnlineAsync(partnerId);
            var lastMessage = room.Messages.FirstOrDefault();

            dtos.Add(new ChatRoomResponseDTO
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
            });
        }

        return dtos;
    }
}
