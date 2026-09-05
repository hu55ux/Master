using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Chat.Queries.GetRoomMessages;

public record GetRoomMessagesQuery(Guid RoomId, int Page = 1, int PageSize = 20) : IRequest<PagedResult<ChatMessageResponseDTO>>;

public class GetRoomMessagesHandler : IRequestHandler<GetRoomMessagesQuery, PagedResult<ChatMessageResponseDTO>>
{
    private readonly IChatRepository _chatRepository;

    public GetRoomMessagesHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<PagedResult<ChatMessageResponseDTO>> Handle(GetRoomMessagesQuery request, CancellationToken cancellationToken)
    {
        var pagedMessages = await _chatRepository.GetRoomMessagesPagedAsync(request.RoomId, request.Page, request.PageSize, cancellationToken);

        var dtos = pagedMessages.Items.Select(msg => new ChatMessageResponseDTO
        {
            Id = msg.Id,
            ChatRoomId = msg.ChatRoomId,
            SenderId = msg.SenderId,
            SenderName = msg.Sender != null ? $"{msg.Sender.FirstName} {msg.Sender.LastName}".Trim() : string.Empty,
            MessageText = msg.MessageText,
            Type = msg.Type.ToString(),
            ProductId = msg.ProductId,
            ProductName = msg.ProductName,
            ProductPrice = msg.ProductPrice,
            ProductImageUrl = msg.ProductImageUrl,
            SentAt = msg.SentAt,
            IsRead = msg.IsRead
        }).ToList();

        return PagedResult<ChatMessageResponseDTO>.Create(dtos, pagedMessages.Page, pagedMessages.PageSize, pagedMessages.TotalCount);
    }
}
