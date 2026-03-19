using Master.Application.Common;
using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Skills.Queries.GetPagedResult;

/// <summary>
/// Query for retrieving a paginated and filtered list of skills.
/// </summary>
/// <param name="Query">The filtering, sorting, and pagination parameters.</param>
public record GetPagedSkillsQuery(SkillQuery Query) : IRequest<PagedResult<SkillResponseDTO>>;