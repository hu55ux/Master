using MediatR;
using Master.Application.DTOs;
namespace Master.Application.Features.Skills.Queries.GetSkillsByUserId;

public record GetSkillsByUserIdQuery(Guid UserId) : IRequest<IEnumerable<SkillResponseDTO>>;
