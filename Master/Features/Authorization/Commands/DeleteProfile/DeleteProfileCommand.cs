using MediatR;

namespace Master.Features.Authorization.Commands.DeleteProfile;

public record DeleteProfileCommand(Guid UserId) : IRequest<bool>;
