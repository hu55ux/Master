using Master.Application.DTOs;
using MediatR;
namespace Master.Application.Features.Authorization.Commands.ChangePassword;

public record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Request) : IRequest<AuthResponseDTO>;