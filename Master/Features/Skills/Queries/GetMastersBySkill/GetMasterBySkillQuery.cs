using Master.DTOs;
using MediatR;

namespace Master.Features.Skills.Queries.GetMastersBySkill;

public record GetMastersBySkillQuery(Guid SkillId) : IRequest<IEnumerable<AuthResponseDTO>>;
