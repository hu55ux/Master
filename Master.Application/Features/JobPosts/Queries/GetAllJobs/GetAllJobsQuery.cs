using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetAllJobs;

public record GetAllJobsQuery() : IRequest<IEnumerable<JobPostResponseDTO>>;

