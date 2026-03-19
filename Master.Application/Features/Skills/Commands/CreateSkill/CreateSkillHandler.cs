using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Application.Models;
using MediatR;

namespace Master.Application.Features.Skills.Commands.CreateSkill;
public class CreateSkillHandler : IRequestHandler<CreateSkillCommand, SkillResponseDTO>
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public CreateSkillHandler(ISkillRepository skillRepository, IMapper mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    public async Task<SkillResponseDTO> Handle(CreateSkillCommand command, CancellationToken ct)
    {
        var nameExists = await _skillRepository.ExistsByNameAsync(command.Request.Name, ct);

        if (nameExists)
            throw new InvalidOperationException($"Skill '{command.Request.Name}' already exists.");

        var newSkill = _mapper.Map<Skill>(command.Request);

        await _skillRepository.AddAsync(newSkill, ct);
        await _skillRepository.SaveChangesAsync(ct);

        return _mapper.Map<SkillResponseDTO>(newSkill);
    }
}