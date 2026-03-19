using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Skills.Commands.UpdateSkill;

public class UpdateSkillHandler : IRequestHandler<UpdateSkillCommand, SkillResponseDTO>
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public UpdateSkillHandler(ISkillRepository skillRepository, IMapper mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    public async Task<SkillResponseDTO> Handle(UpdateSkillCommand command, CancellationToken ct)
    {
        var skill = await _skillRepository.GetByIdAsync(command.Id, ct);

        if (skill == null)
            throw new KeyNotFoundException($"Skill with ID '{command.Id}' was not found.");

        _mapper.Map(command.Request, skill);

        _skillRepository.Update(skill);
        await _skillRepository.SaveChangesAsync(ct);

        return _mapper.Map<SkillResponseDTO>(skill);
    }
}