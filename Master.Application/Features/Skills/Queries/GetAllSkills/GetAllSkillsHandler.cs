using AutoMapper;
using Master.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.Skills.Queries.GetAllSkills;

public class GetAllSkillsHandler : IRequestHandler<GetAllSkillsQuery, IEnumerable<SkillResponseDTO>>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public GetAllSkillsHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SkillResponseDTO>> Handle(GetAllSkillsQuery request, CancellationToken ct)
    {
        var skills = await _context.Skills
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<SkillResponseDTO>>(skills);
    }
}
