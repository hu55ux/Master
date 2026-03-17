using Master.DTOs;
using MediatR;

namespace Master.Features.Skills.Commands.CreateSkill;

public record CreateSkillCommand(CreateSkillDTO Request) : IRequest<SkillResponseDTO>;
