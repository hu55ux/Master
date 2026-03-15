using MediatR;

namespace Master.Features.Authorization.Commands;

public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;