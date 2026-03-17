using MediatR;

namespace Master.Features.Skills.Commands.AssignSkills;

public record AssignSkillsToMasterCommand(Guid MasterId, List<Guid> SkillIds) : IRequest<bool>;
