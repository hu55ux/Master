using AutoMapper;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.Skills.Queries.GetPagedResult;

public class GetPagedSkillsHandler : IRequestHandler<GetPagedSkillsQuery, PagedResult<SkillResponseDTO>>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public GetPagedSkillsHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<SkillResponseDTO>> Handle(GetPagedSkillsQuery request, CancellationToken ct)
    {
        request.Query.Validate();

        var skillQuery = _context.Skills.Include(s => s.UserSkills).AsNoTracking();

        if (!string.IsNullOrEmpty(request.Query.SearchTerm))
        {
            var term = request.Query.SearchTerm.Trim().ToLower();
            skillQuery = skillQuery.Where(q => q.Name.ToLower().Contains(term) || q.Description.ToLower().Contains(term));
        }

        skillQuery = ApplySorting(skillQuery, request.Query.Sort, request.Query.SortDirection);

        var totalCount = await skillQuery.CountAsync(ct);
        var skip = (request.Query.Page - 1) * request.Query.PageSize;

        var skills = await skillQuery
            .Skip(skip)
            .Take(request.Query.PageSize)
            .ToListAsync(ct);

        return PagedResult<SkillResponseDTO>.Create(
            _mapper.Map<IEnumerable<SkillResponseDTO>>(skills),
            request.Query.Page,
            request.Query.PageSize,
            totalCount);
    }

    private IQueryable<Skill> ApplySorting(IQueryable<Skill> query, string sort, string direction)
    {
        var isDesc = direction?.ToLower() == "desc";
        return sort?.ToLower() switch
        {
            "title" => isDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            "description" => isDesc ? query.OrderByDescending(c => c.Description) : query.OrderBy(c => c.Description),
            _ => query.OrderByDescending(c => c.Name)
        };
    }
}
