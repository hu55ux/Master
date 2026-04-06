using Master.Application.Interfaces;
using Master.Domain.Models;
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
        var job = await _jobRepository.GetByIdAndCustomerIdAsync(command.JobId, command.CustomerId, ct);

        if (job == null) throw new KeyNotFoundException("Job post not found for the given JobId and CustomerId.");

        bool isValidTransition = (job.JPStatus, command.NewStatus) switch
        {
            (JobPostStatus.Pending, JobPostStatus.Active) => true,
            (JobPostStatus.Pending, JobPostStatus.Canceled) => true,
            (JobPostStatus.Active, JobPostStatus.InProgress) => true,
            (JobPostStatus.InProgress, JobPostStatus.Completed) => true,
            _ => false
        };

        if (!isValidTransition)
            throw new InvalidOperationException($"{job.JPStatus} statusundan {command.NewStatus} statusuna keçid mümkün deyil.");

        job.JPStatus = command.NewStatus;
        _jobRepository.Update(job);
        return await _jobRepository.SaveChangesAsync(ct);
    }
}