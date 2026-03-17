using Master.DTOs;
using MediatR;

namespace Master.Features.Skills.Queries.GetAllSkills;

public record GetAllSkillsQuery() : IRequest<IEnumerable<SkillResponseDTO>>;
