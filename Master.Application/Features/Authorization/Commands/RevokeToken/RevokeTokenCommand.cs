using MediatR;

namespace Master.Application.Features.Authorization.Commands.RevokeToken;

public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;