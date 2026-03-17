using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Queries.GetAllJobs;

public record GetAllJobsQuery() : IRequest<IEnumerable<JobPostResponseDTO>>;

