using Master.Models;
using MediatR;

namespace Master.Features.Authorization.Queries.GetUserEntity;

public record GetUserEntityQuery(Guid UserId) : IRequest<AppUser>;
