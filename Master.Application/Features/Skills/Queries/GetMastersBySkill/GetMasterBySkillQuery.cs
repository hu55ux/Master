using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Skills.Queries.GetMastersBySkill;

public record GetMastersBySkillQuery(Guid SkillId) : IRequest<IEnumerable<AuthResponseDTO>>;
