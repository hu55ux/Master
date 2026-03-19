using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetAllJobs;

public class GetAllJobsHandler : IRequestHandler<GetAllJobsQuery, IEnumerable<JobPostResponseDTO>>
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;

    public GetAllJobsHandler(IJobPostRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JobPostResponseDTO>> Handle(GetAllJobsQuery request, CancellationToken ct)
    {
        var jobs = await _jobRepository.GetAllWithDetailsAsync(ct);

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }
}