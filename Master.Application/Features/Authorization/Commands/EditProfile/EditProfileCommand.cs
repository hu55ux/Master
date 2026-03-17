using Master.Application.DTOs;
using MediatR;
namespace Master.Application.Features.Authorization.Commands.EditProfile;

public record EditProfileCommand(Guid UserId, ProfileEditRequest Request) : IRequest<AuthResponseDTO>;
