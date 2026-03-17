using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Skills.Commands.CreateSkill;

public record CreateSkillCommand(CreateSkillDTO Request) : IRequest<SkillResponseDTO>;
