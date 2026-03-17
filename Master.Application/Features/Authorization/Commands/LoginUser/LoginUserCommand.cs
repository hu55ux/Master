using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Authorization.Commands.LoginUser;

public record LoginUserCommand(LoginRequest Request) : IRequest<AuthResponseDTO>;