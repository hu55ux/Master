using AutoMapper;
using Master.Data;
using Master.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Features.Skills.Queries.GetMastersBySkill;

public class GetMastersBySkillHandler : IRequestHandler<GetMastersBySkillQuery, IEnumerable<AuthResponseDTO>>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public GetMastersBySkillHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AuthResponseDTO>> Handle(GetMastersBySkillQuery request, CancellationToken ct)
    {
        var masters = await _context.UserSkills
            .AsNoTracking()
            .Where(us => us.SkillId == request.SkillId)
            .Include(us => us.User)
            .Select(us => us.User)
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<AuthResponseDTO>>(masters);
    }
}
