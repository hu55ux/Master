using Master.DTOs;
using MediatR;

namespace Master.Features.Skills.Queries.GetMySkilss;

public record GetMySkillsQuery(Guid UserId) : IRequest<IEnumerable<SkillResponseDTO>>;
