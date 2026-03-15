using Master.Models;
using MediatR;

namespace Master.Features.Authorization.Queries;

public record GetUserEntityQuery(Guid UserId) : IRequest<AppUser>;
