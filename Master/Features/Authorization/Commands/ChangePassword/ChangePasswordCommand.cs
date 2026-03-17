using Master.DTOs;
using MediatR;

namespace Master.Features.Authorization.Commands.ChangePassword;

public record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Request) : IRequest<AuthResponseDTO>;
