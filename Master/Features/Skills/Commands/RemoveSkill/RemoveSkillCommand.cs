using MediatR;

namespace Master.Features.Skills.Commands.RemoveSkill;

public record RemoveSkillCommand(Guid MasterId, Guid SkillId) : IRequest<bool>;
