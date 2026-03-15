using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Queries;

public record GetMyJobsQuery(Guid ClientId) : IRequest<IEnumerable<JobPostResponseDTO>>;
