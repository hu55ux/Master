using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetJobsByUser;

public record GetJobsByUserIdQuery(Guid UserId, bool IsOwner) : IRequest<IEnumerable<JobPostResponseDTO>>;
