using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Skills.Queries.GetAllSkills;

public class GetAllSkillsHandler : IRequestHandler<GetAllSkillsQuery, IEnumerable<SkillResponseDTO>>
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public GetAllSkillsHandler(ISkillRepository skillRepository, IMapper mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SkillResponseDTO>> Handle(GetAllSkillsQuery request, CancellationToken ct)
    {
        var skills = await _skillRepository.GetAllAsync(ct);

        return _mapper.Map<IEnumerable<SkillResponseDTO>>(skills);
    }
}