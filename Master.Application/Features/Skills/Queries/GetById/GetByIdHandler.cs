using AutoMapper;
using Master.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.Skills.Queries.GetById;

public class GetByIdHandler : IRequestHandler<GetByIdQuery, SkillResponseDTO>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public GetByIdHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SkillResponseDTO> Handle(GetByIdQuery request, CancellationToken ct)
    {
        var skill = await _context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (skill == null)
            throw new KeyNotFoundException($"Skill with ID '{request.Id}' was not found.");

        return _mapper.Map<SkillResponseDTO>(skill);
    }
}
