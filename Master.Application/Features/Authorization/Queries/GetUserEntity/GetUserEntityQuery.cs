using Master.Application.Models;
using MediatR;

namespace Master.Application.Features.Authorization.Queries.GetUserEntity;

public record GetUserEntityQuery(Guid UserId) : IRequest<AppUser>;
