using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Authorization.Commands.RefreshToken;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<AuthResponseDTO>;