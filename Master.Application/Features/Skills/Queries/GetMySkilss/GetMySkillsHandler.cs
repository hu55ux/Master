using AutoMapper;
using Master.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.Skills.Queries.GetMySkilss;

public class GetMySkillsHandler : IRequestHandler<GetMySkillsQuery, IEnumerable<SkillResponseDTO>>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public GetMySkillsHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SkillResponseDTO>> Handle(GetMySkillsQuery request, CancellationToken ct)
    {
        var mySkills = await _context.UserSkills
            .AsNoTracking()
            .Where(us => us.UserId == request.UserId)
            .Include(us => us.Skill) 
            .Select(us => us.Skill)  
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<SkillResponseDTO>>(mySkills);
    }
}
