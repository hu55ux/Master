using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetMyJobs;

public class GetMyJobsHandler : IRequestHandler<GetMyJobsQuery, IEnumerable<JobPostResponseDTO>>
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;

    public GetMyJobsHandler(IJobPostRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JobPostResponseDTO>> Handle(GetMyJobsQuery request, CancellationToken ct)
    {
        var jobs = await _jobRepository.GetJobsByCustomerIdAsync(request.ClientId, ct);

        if (jobs == null || !jobs.Any())
            return Enumerable.Empty<JobPostResponseDTO>();

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }
}