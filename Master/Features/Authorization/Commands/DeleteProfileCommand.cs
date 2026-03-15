using MediatR;

namespace Master.Features.Authorization.Commands;

public record DeleteProfileCommand(Guid UserId) : IRequest<bool>;
