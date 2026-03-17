using Master.DTOs;
using MediatR;

namespace Master.Features.Authorization.Commands.RefreshToken;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<AuthResponseDTO>;