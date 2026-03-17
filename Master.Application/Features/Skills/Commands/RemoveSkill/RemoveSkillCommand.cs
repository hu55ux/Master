using MediatR;
namespace Master.Application.Features.Skills.Commands.RemoveSkill;

public record RemoveSkillCommand(Guid MasterId, Guid SkillId) : IRequest<bool>;
