using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetActiveJobs;

public record GetActiveJobsBySkillQuery(Guid SkillId) : IRequest<IEnumerable<JobPostResponseDTO>>;
