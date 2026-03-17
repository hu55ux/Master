using AutoMapper;
using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Skills.Commands.UpdateSkill;

public class UpdateSkillHandler : IRequestHandler<UpdateSkillCommand, SkillResponseDTO>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public UpdateSkillHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SkillResponseDTO> Handle(UpdateSkillCommand command, CancellationToken ct)
    {
        var skill = await _context.Skills.FindAsync(command.Id, ct);
        if (skill == null) throw new KeyNotFoundException("Skill not found.");

        _mapper.Map(command.Request, skill);
        await _context.SaveChangesAsync(ct);
        return _mapper.Map<SkillResponseDTO>(skill);
    }
}
