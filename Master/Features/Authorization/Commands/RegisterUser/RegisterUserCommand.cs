using Master.DTOs;
using MediatR;
namespace Master.Features.Authorization.Commands.RegisterUser;

public record RegisterUserCommand(RegisterRequest Request) : IRequest<AuthResponseDTO>;