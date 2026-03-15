using Master.DTOs;
using MediatR;
namespace Master.Features.Authorization.Commands;

public record RegisterUserCommand(RegisterRequest Request) : IRequest<AuthResponseDTO>;