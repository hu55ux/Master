using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Skills.Queries.GetMastersBySkill;

public class GetMastersBySkillHandler : IRequestHandler<GetMastersBySkillQuery, IEnumerable<AuthResponseDTO>>
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public GetMastersBySkillHandler(ISkillRepository skillRepository, IMapper mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the retrieval of all master users associated with a specific skill.
    /// </summary>
    /// <param name="request">The query containing the skill ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of master users mapped to AuthResponseDTO.</returns>
    public async Task<IEnumerable<AuthResponseDTO>> Handle(GetMastersBySkillQuery request, CancellationToken ct)
    {
        var masters = await _skillRepository.GetMastersBySkillAsync(request.SkillId, ct);

        return _mapper.Map<IEnumerable<AuthResponseDTO>>(masters);
    }
}
