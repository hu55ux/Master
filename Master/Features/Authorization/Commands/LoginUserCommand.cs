using Master.DTOs;
using MediatR;

namespace Master.Features.Authorization.Commands;

public record LoginUserCommand(LoginRequest Request) : IRequest<AuthResponseDTO>;