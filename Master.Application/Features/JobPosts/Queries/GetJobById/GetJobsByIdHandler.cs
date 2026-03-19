using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetJobById;

public class GetJobByIdHandler : IRequestHandler<GetJobByIdQuery, JobPostResponseDTO>
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;

    public GetJobByIdHandler(IJobPostRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<JobPostResponseDTO> Handle(GetJobByIdQuery request, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdWithDetailsAsync(request.Id, ct);

        if (job == null)
            throw new KeyNotFoundException($"Job with ID '{request.Id}' was not found.");

        return _mapper.Map<JobPostResponseDTO>(job);
    }
}