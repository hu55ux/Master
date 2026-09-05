using Master.Application.Interfaces;
using Master.Domain.Models;
using Master.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Master.Infrastructure.Repositories;

public class JobOfferRepository : IJobOfferRepository
{
    private readonly MasterDbContext _context;

    public JobOfferRepository(MasterDbContext context) => _context = context;

    public async Task<JobOffer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.JobOffers.FindAsync(new object[] { id }, ct);

    public async Task<JobOffer?> GetWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.JobOffers
            .Include(o => o.JobPost)
            .Include(o => o.Master)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<List<JobOffer>> GetByJobPostIdAsync(Guid jobPostId, CancellationToken ct = default)
    {
        return await _context.JobOffers
            .Include(o => o.Master)
            .Include(o => o.Customer)
            .Where(o => o.JobPostId == jobPostId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<JobOffer>> GetByMasterIdAsync(Guid masterId, CancellationToken ct = default)
    {
        return await _context.JobOffers
            .Include(o => o.JobPost)
            .Include(o => o.Customer)
            .Where(o => o.MasterId == masterId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<JobOffer>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.JobOffers
            .Include(o => o.JobPost)
            .Include(o => o.Master)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(JobOffer offer, CancellationToken ct = default)
        => await _context.JobOffers.AddAsync(offer, ct);

    public void Update(JobOffer offer)
        => _context.JobOffers.Update(offer);

    public void Remove(JobOffer offer)
        => _context.JobOffers.Remove(offer);

    public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct) > 0;
}
