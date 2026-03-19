using Master.Application.Interfaces;
using Master.Features.JobPosts.Commands.DeleteJob;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.DeleteJob;

public class DeleteJobHandler : IRequestHandler<DeleteJobCommand, bool>
{
    private readonly IJobPostRepository _jobRepository;

    public DeleteJobHandler(IJobPostRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<bool> Handle(DeleteJobCommand command, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAndCustomerIdAsync(command.JobId, command.ClientId, ct);

        if (job == null)
        {
            throw new KeyNotFoundException($"Job not found or unauthorized: {command.JobId}");
        }

        _jobRepository.Remove(job);

        return await _jobRepository.SaveChangesAsync(ct);
    }
}