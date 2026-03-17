using Master.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Master.Infrastructure.BackgroundJobs;

public class TokenCleanupJob
{
    private readonly MasterDbContext _context;

    public TokenCleanupJob(MasterDbContext context)
    {
        _context = context;
    }

    public async Task DeleteRevokedTokens()
    {
        var threshold = DateTime.UtcNow.AddDays(-7);

        await _context.RefreshTokens
            .Where(t => t.RevokedAt != null && t.RevokedAt < threshold)
            .ExecuteDeleteAsync();
    }
}