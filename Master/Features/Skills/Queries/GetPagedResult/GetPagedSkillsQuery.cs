using Master.Common;
using Master.DTOs;
using MediatR;

namespace Master.Features.Skills.Queries.GetPagedResult;

public record GetPagedSkillsQuery(SkillQuery Query) : IRequest<PagedResult<SkillResponseDTO>>;