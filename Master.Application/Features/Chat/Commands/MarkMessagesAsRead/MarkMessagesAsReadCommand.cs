using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Chat.Commands.MarkMessagesAsRead;

public record MarkMessagesAsReadCommand(Guid RoomId, Guid CurrentUserId) : IRequest<int>;

public class MarkMessagesAsReadHandler : IRequestHandler<MarkMessagesAsReadCommand, int>
{
    private readonly IChatRepository _chatRepository;

    public MarkMessagesAsReadHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<int> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        return await _chatRepository.MarkMessagesAsReadAsync(request.RoomId, request.CurrentUserId, cancellationToken);
    }
}
