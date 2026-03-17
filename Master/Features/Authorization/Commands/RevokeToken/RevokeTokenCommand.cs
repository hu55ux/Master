using MediatR;

namespace Master.Features.Authorization.Commands.RevokeToken;

public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;