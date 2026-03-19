using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;


namespace Master.Application.Features.JobPosts.Queries.GetActiveJobs;

public class GetActiveJobsBySkillHandler : IRequestHandler<GetActiveJobsBySkillQuery, IEnumerable<JobPostResponseDTO>>
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;

    public GetActiveJobsBySkillHandler(IJobPostRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JobPostResponseDTO>> Handle(GetActiveJobsBySkillQuery request, CancellationToken ct)
    {
        var jobs = await _jobRepository.GetActiveJobsBySkillAsync(request.SkillId, ct);

        if (jobs == null || !jobs.Any())
            return Enumerable.Empty<JobPostResponseDTO>();

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }
}