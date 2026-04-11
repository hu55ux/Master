using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.CreateJob;

/// <summary>
/// Handler for the <see cref="CreateJobCommand"/>.
/// Maps the request to a job entity and persists it to the database.
/// </summary>
public class CreateJobHandler : IRequestHandler<CreateJobCommand, JobPostResponseDTO>
{
    private readonly IJobPostRepository _jobRepository;
    private readonly IMapper _mapper;

    public CreateJobHandler(IJobPostRepository jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the job creation process.
    /// </summary>
    /// <param name="command">The create job command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="JobPostResponseDTO"/> representing the created job.</returns>
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