using MediatR;
using Master.Application.DTOs;
namespace Master.Application.Features.Skills.Queries.GetMySkilss;

public record GetMySkillsQuery(Guid UserId) : IRequest<IEnumerable<SkillResponseDTO>>;
