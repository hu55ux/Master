using Master.Application.Common;
using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Authorization.Queries.GetClientsList;

public record GetClientsListQuery(UserQuery Query) : IRequest<PagedResult<AuthResponseDTO>>;
