using Master.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Features.JobPosts.Commands;

public class DeleteJobHandler : IRequestHandler<DeleteJobCommand, bool>
{
    private readonly MasterDbContext _context;

    public DeleteJobHandler(MasterDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteJobCommand command, CancellationToken ct)
    {
        var job = await _context.JobPosts
            .FirstOrDefaultAsync(j => j.Id == command.JobId && j.CustomerId == command.ClientId, ct);

        if (job == null)
        {
            throw new KeyNotFoundException($"Job not found or unauthorized: {command.JobId}");
        }

        _context.JobPosts.Remove(job);

        var result = await _context.SaveChangesAsync(ct);

        return result > 0;
    }
}
