using Master.Domain.Models;

namespace Master.Application.Interfaces;

/// <summary>
/// Repository interface for Job Offer management.
/// </summary>
public interface IJobOfferRepository
{
    Task<JobOffer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<JobOffer?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<List<JobOffer>> GetByJobPostIdAsync(Guid jobPostId, CancellationToken ct = default);
    Task<List<JobOffer>> GetByMasterIdAsync(Guid masterId, CancellationToken ct = default);
    Task<List<JobOffer>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task AddAsync(JobOffer offer, CancellationToken ct = default);
    void Update(JobOffer offer);
    void Remove(JobOffer offer);
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
}
