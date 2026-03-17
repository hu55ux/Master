using Master.Application.DTOs;
using MediatR;
namespace Master.Application.Features.Authorization.Commands.RegisterUser;

public record RegisterUserCommand(RegisterRequest Request) : IRequest<AuthResponseDTO>;