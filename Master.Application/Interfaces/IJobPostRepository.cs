using Master.Application.Models;
namespace Master.Application.Interfaces;

public interface IJobPostRepository
{
    Task<JobPost?> GetByIdAndCustomerIdAsync(Guid jobId, Guid customerId, CancellationToken ct);
    void Update(JobPost job);
    Task AddAsync(JobPost job, CancellationToken ct);
    Task LoadReferencesAsync(JobPost job, CancellationToken ct);
    void Remove(JobPost job);
    Task<bool> SaveChangesAsync(CancellationToken ct);
    Task<JobPost?> GetWithDetailsAsync(Guid jobId, Guid customerId, CancellationToken ct);
    Task<bool> SkillExistsAsync(Guid skillId, CancellationToken ct);
    Task<IEnumerable<JobPost>> GetActiveJobsBySkillAsync(Guid skillId, CancellationToken ct);
    Task<IEnumerable<JobPost>> GetAllWithDetailsAsync(CancellationToken ct);
    Task<JobPost?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<JobPost>> GetJobsByCustomerIdAsync(Guid customerId, CancellationToken ct);
    Task<(IEnumerable<JobPost> Items, int TotalCount)> GetPagedJobsAsync(JobPostQuery query, CancellationToken ct);
    Task<IEnumerable<JobPost>> GetJobsByUserIdAsync(Guid userId, bool onlyActive, CancellationToken ct);
    Task<AppUser?> GetCustomerByJobIdAsync(Guid jobId, CancellationToken ct);
}
