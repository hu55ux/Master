using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetMyJobs;

public record GetMyJobsQuery(Guid ClientId) : IRequest<IEnumerable<JobPostResponseDTO>>;
