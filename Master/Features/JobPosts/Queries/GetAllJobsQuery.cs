using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Queries;

public record GetAllJobsQuery() : IRequest<IEnumerable<JobPostResponseDTO>>;

