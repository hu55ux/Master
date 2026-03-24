using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Authorization.Queries.GetUserById;

public record GetUserByIdQuery(Guid id) : IRequest<AuthResponseDTO>
{
}
