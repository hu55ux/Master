using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Queries.GetActiveJobs;

public record GetActiveJobsBySkillQuery(Guid SkillId) : IRequest<IEnumerable<JobPostResponseDTO>>;
