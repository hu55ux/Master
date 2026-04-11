using Master.Application.Common;
using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Authorization.Queries.GetMastersList;

public record GetMastersListQuery(UserQuery Query) : IRequest<PagedResult<AuthResponseDTO>>;
