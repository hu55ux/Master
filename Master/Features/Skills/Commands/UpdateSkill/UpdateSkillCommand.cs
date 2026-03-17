using Master.DTOs;
using MediatR;

namespace Master.Features.Skills.Commands.UpdateSkill;

public record UpdateSkillCommand(Guid Id, UpdateSkillDTO Request) : IRequest<SkillResponseDTO>;
