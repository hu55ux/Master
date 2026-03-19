using AutoMapper;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetPagedJobs;

public class GetPagedJobPostsHandler : IRequestHandler<GetPagedJobPostsQuery, PagedResult<JobPostResponseDTO>>
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;

    public GetPagedJobPostsHandler(IJobPostRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<JobPostResponseDTO>> Handle(GetPagedJobPostsQuery request, CancellationToken ct)
    {
        request.Query.Validate();

        var (items, totalCount) = await _jobRepository.GetPagedJobsAsync(request.Query, ct);

        var mappedItems = _mapper.Map<IEnumerable<JobPostResponseDTO>>(items);

        return PagedResult<JobPostResponseDTO>.Create(
            mappedItems,
            request.Query.Page,
            request.Query.PageSize,
            totalCount);
    }
}
