using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetUserByJob;

public record GetUserByJobIdQuery(Guid JobId) : IRequest<AuthResponseDTO>;
