using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Skills.Queries.GetMySkilss;

public class GetMySkillsHandler : IRequestHandler<GetMySkillsQuery, IEnumerable<SkillResponseDTO>>
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public GetMySkillsHandler(ISkillRepository skillRepository, IMapper mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the request to retrieve all skills associated with the currently authenticated user.
    /// </summary>
    /// <param name="request">The query containing the user identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of skill data transfer objects for the user.</returns>
    public async Task<IEnumerable<SkillResponseDTO>> Handle(GetMySkillsQuery request, CancellationToken ct)
    {
        var mySkills = await _skillRepository.GetSkillsByUserIdAsync(request.UserId, ct);

        return _mapper.Map<IEnumerable<SkillResponseDTO>>(mySkills);
    }
}