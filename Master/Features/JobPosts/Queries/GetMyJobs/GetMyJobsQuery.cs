using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Queries.GetMyJobs;

public record GetMyJobsQuery(Guid ClientId) : IRequest<IEnumerable<JobPostResponseDTO>>;
