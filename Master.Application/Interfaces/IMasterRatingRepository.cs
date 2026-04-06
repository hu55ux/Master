using Master.Domain.Models;

namespace Master.Application.Interfaces;

public interface IMasterRatingRepository
{
    Task AddAsync(MasterRating rating, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct); 
    Task<bool> AlreadyRatedAsync(Guid masterId, Guid clientId, CancellationToken ct);
    Task<MasterRating?> GetAsync(Guid masterId, Guid customerId, CancellationToken ct);
    Task DeleteAsync(MasterRating rating, CancellationToken ct);
    Task UpdateMasterStatsAsync(Guid masterId, CancellationToken ct);
    Task<IEnumerable<MasterRating>> GetByMasterIdAsync(Guid masterId, CancellationToken ct);
}
