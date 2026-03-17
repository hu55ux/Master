using MediatR;

namespace Master.Application.Features.Authorization.Commands.DeleteProfile;

public record DeleteProfileCommand(Guid UserId) : IRequest<bool>;
