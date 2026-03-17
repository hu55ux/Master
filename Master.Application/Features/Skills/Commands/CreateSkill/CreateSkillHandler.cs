using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.Skills.Commands.CreateSkill;
public class CreateSkillHandler : IRequestHandler<CreateSkillCommand, SkillResponseDTO>
{
    private readonly MasterDbContext _context;
    private readonly IMapper _mapper;

    public CreateSkillHandler(MasterDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SkillResponseDTO> Handle(CreateSkillCommand command, CancellationToken ct)
    {
        var nameExists = await _context.Skills
            .AnyAsync(s => s.Name.ToLower() == command.Request.Name.ToLower(), ct);

        if (nameExists)
            throw new InvalidOperationException($"Skill '{command.Request.Name}' already exists.");

        var newSkill = _mapper.Map<Skill>(command.Request);
        _context.Skills.Add(newSkill);
        await _context.SaveChangesAsync(ct);

        return _mapper.Map<SkillResponseDTO>(newSkill);
    }
}