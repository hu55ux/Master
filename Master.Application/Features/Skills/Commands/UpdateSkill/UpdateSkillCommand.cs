using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Skills.Commands.UpdateSkill;

public record UpdateSkillCommand(Guid Id, UpdateSkillDTO Request) : IRequest<SkillResponseDTO>;
