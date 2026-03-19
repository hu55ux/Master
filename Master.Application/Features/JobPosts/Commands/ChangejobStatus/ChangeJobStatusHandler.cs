using Master.Application.Interfaces;
using Master.Application.Models;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.ChangejobStatus;

public class ChangeJobStatusHandler : IRequestHandler<ChangeJobStatusCommand, bool>
{
    private readonly IJobPostRepository _jobRepository;

    public ChangeJobStatusHandler(IJobPostRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<bool> Handle(ChangeJobStatusCommand command, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAndCustomerIdAsync(command.JobId, command.ClientId, ct);

        if (job == null)
            throw new KeyNotFoundException($"Job not found or unauthorized: {command.JobId}");

        if (job.JPStatus == JobPostStatus.Completed || job.JPStatus == JobPostStatus.Canceled)
            throw new InvalidOperationException("Cannot change status of a completed or canceled job.");

        job.JPStatus = command.NewStatus;

        _jobRepository.Update(job);
        return await _jobRepository.SaveChangesAsync(ct);
    }
}