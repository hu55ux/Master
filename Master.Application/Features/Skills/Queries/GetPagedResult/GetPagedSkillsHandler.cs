using AutoMapper;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Skills.Queries.GetPagedResult;

public class GetPagedSkillsHandler : IRequestHandler<GetPagedSkillsQuery, PagedResult<SkillResponseDTO>>
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public GetPagedSkillsHandler(ISkillRepository skillRepository, IMapper mapper)
    {
        _skillRepository = skillRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the request to retrieve a paged and filtered list of skills.
    /// </summary>
    public async Task<PagedResult<SkillResponseDTO>> Handle(GetPagedSkillsQuery request, CancellationToken ct)
    {
        request.Query.Validate();

        var (items, totalCount) = await _skillRepository.GetPagedAsync(request.Query, ct);

        var mappedItems = _mapper.Map<IEnumerable<SkillResponseDTO>>(items);

        return PagedResult<SkillResponseDTO>.Create(
            mappedItems,
            request.Query.Page,
            request.Query.PageSize,
            totalCount);
    }
}
