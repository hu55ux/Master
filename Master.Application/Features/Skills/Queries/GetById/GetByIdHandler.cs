using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Skills.Queries.GetById;

public class GetByIdHandler : IRequestHandler<GetByIdQuery, SkillResponseDTO>
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public GetByIdHandler(ISkillRepository skillRepository, IMapper mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the retrieval of a skill by its unique identifier.
    /// </summary>
    /// <param name="request">The query containing the skill ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A data transfer object representing the requested skill.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the skill with the given ID does not exist.</exception>
    public async Task<SkillResponseDTO> Handle(GetByIdQuery request, CancellationToken ct)
    {
        var skill = await _skillRepository.GetByIdAsync(request.Id, ct);

        if (skill == null)
            throw new KeyNotFoundException($"Skill with ID '{request.Id}' was not found.");

        return _mapper.Map<SkillResponseDTO>(skill);
    }
}