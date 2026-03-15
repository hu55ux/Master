using Master.DTOs;
using MediatR;

namespace Master.Features.Authorization.Commands;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<AuthResponseDTO>;