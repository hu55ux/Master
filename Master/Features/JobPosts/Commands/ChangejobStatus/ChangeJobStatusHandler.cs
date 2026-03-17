using Master.Data;
using Master.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Features.JobPosts.Commands.ChangejobStatus;

public class ChangeJobStatusHandler : IRequestHandler<ChangeJobStatusCommand, bool>
{
    private readonly MasterDbContext _context;

    public ChangeJobStatusHandler(MasterDbContext context) => _context = context;

    public async Task<bool> Handle(ChangeJobStatusCommand command, CancellationToken ct)
    {
        var job = await _context.JobPosts
            .FirstOrDefaultAsync(j => j.Id == command.JobId && j.CustomerId == command.ClientId, ct);

        if (job == null) throw new KeyNotFoundException($"Job not found or unauthorized: {command.JobId}");

        if (job.JPStatus == JobPostStatus.Completed || job.JPStatus == JobPostStatus.Canceled)
            throw new InvalidOperationException("Cannot change status of a completed or canceled job.");

        job.JPStatus = command.NewStatus;
        return await _context.SaveChangesAsync(ct) > 0;
    }
}
