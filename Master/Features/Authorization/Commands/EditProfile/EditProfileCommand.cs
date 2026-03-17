using Master.DTOs;
using MediatR;

namespace Master.Features.Authorization.Commands.EditProfile;

public record EditProfileCommand(Guid UserId, ProfileEditRequest Request) : IRequest<AuthResponseDTO>;
