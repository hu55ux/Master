using System;
using Master.Application.Interfaces;
using Master.Domain.Models;
using Master.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Master.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for master rating related operations.
/// </summary>
public class MasterRatingRepository : IMasterRatingRepository
{
    private readonly MasterDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MasterRatingRepository"/> class.
    /// </summary>
    public MasterRatingRepository(MasterDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(MasterRating rating, CancellationToken ct)
    {
        await _context.MasterRatings.AddAsync(rating, ct);
    }

    /// <inheritdoc />
    public async Task<bool> AlreadyRatedAsync(Guid masterId, Guid clientId, CancellationToken ct)
    {
        return await _context.MasterRatings
            .AnyAsync(r => r.MasterId == masterId && r.CustomerId == clientId, ct);
    }

    public async Task<MasterRating?> GetAsync(Guid masterId, Guid customerId, CancellationToken ct)
    {
        return await _context.MasterRatings
            .FirstOrDefaultAsync(r => r.MasterId == masterId && r.CustomerId == customerId, ct);
    }

    public Task DeleteAsync(MasterRating rating, CancellationToken ct)
    {
        _context.MasterRatings.Remove(rating);
        return Task.CompletedTask;
    }

    public async Task UpdateMasterStatsAsync(Guid masterId, CancellationToken ct)
    {
        var ratings = await _context.MasterRatings
            .Where(r => r.MasterId == masterId)
            .ToListAsync(ct);

        var master = await _context.Users.FindAsync(new object[] { masterId }, ct);
        if (master == null)
            throw new KeyNotFoundException("Master user not found.");

        master.RatingCount = ratings.Count;
        master.AverageRating = ratings.Any() ? (decimal)ratings.Average(r => (double)r.Score) : 0;
        master.UpdatedAt = DateTimeOffset.UtcNow;
    }

    public async Task<IEnumerable<MasterRating>> GetByMasterIdAsync(Guid masterId, CancellationToken ct)
    {
        return await _context.MasterRatings
            .Include(r => r.Customer)
            .Include(r => r.Master)
            .Where(r => r.MasterId == masterId)
            .ToListAsync(ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
