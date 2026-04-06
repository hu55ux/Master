using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.CreateJob;

public class CreateJobHandler : IRequestHandler<CreateJobCommand, JobPostResponseDTO>
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;

    public CreateJobHandler(IJobPostRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<JobPostResponseDTO> Handle(CreateJobCommand command, CancellationToken ct)
    {
        var job = _mapper.Map<JobPost>(command.Request);
        job.CustomerId = command.ClientId;
        job.JPStatus = JobPostStatus.Active;

        await _jobRepository.AddAsync(job, ct);
        await _jobRepository.SaveChangesAsync(ct);

        await _jobRepository.LoadReferencesAsync(job, ct);

        return _mapper.Map<JobPostResponseDTO>(job);
    }
}