using Master.DTOs;
using MediatR;

namespace Master.Features.JobPosts.Queries;

public record GetActiveJobsBySkillQuery(Guid SkillId) : IRequest<IEnumerable<JobPostResponseDTO>>;
