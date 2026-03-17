using Master.DTOs;
using MediatR;

namespace Master.Features.Authorization.Commands.LoginUser;

public record LoginUserCommand(LoginRequest Request) : IRequest<AuthResponseDTO>;