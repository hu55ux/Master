using Master.Application.Common;
using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Skills.Queries.GetPagedResult;

public record GetPagedSkillsQuery(SkillQuery Query) : IRequest<PagedResult<SkillResponseDTO>>;