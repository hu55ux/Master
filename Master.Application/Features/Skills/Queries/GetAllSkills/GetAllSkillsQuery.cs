using Master.Application.DTOs;
using MediatR;
namespace Master.Application.Features.Skills.Queries.GetAllSkills;

public record GetAllSkillsQuery() : IRequest<IEnumerable<SkillResponseDTO>>;
