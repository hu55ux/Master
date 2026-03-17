using MediatR;
namespace Master.Application.Features.Skills.Commands.AssignSkills;

public record AssignSkillsToMasterCommand(Guid MasterId, List<Guid> SkillIds) : IRequest<bool>;
