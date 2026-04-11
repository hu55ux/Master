using Master.Domain.Models;
namespace Master.Application.Interfaces;

/// <summary>
/// Interface for repository operations related to job posts.
/// </summary>
public interface IJobPostRepository
{
    /// <summary>
    /// Retrieves a job post by its ID and the owner's customer ID.
    /// </summary>
    Task<JobPost?> GetByIdAndCustomerIdAsync(Guid jobId, Guid customerId, CancellationToken ct);

    /// <summary>
    /// Marks a job post for update.
    /// </summary>
    void Update(JobPost job);

    /// <summary>
    /// Adds a new job post to the database.
    /// </summary>
    Task AddAsync(JobPost job, CancellationToken ct);

    /// <summary>
    /// Loads navigation properties for a specific job post.
    /// </summary>
    Task LoadReferencesAsync(JobPost job, CancellationToken ct);

    /// <summary>
    /// Marks a job post for removal.
    /// </summary>
    void Remove(JobPost job);

    /// <summary>
    /// Persists all changes to the database.
    /// </summary>
    Task<bool> SaveChangesAsync(CancellationToken ct);

    /// <summary>
    /// Retrieves a job post with all its related details.
    /// </summary>
    Task<JobPost?> GetWithDetailsAsync(Guid jobId, Guid customerId, CancellationToken ct);

    /// <summary>
    /// Checks if a skill with the specified identifier exists.
    /// </summary>
    Task<bool> SkillExistsAsync(Guid skillId, CancellationToken ct);

    /// <summary>
    /// Retrieves active job posts that require a specific skill.
    /// </summary>
    Task<IEnumerable<JobPost>> GetActiveJobsBySkillAsync(Guid skillId, CancellationToken ct);

    /// <summary>
    /// Retrieves all job posts with their related details.
    /// </summary>
    Task<IEnumerable<JobPost>> GetAllWithDetailsAsync(CancellationToken ct);

    /// <summary>
    /// Retrieves a specific job post by ID with its related details.
    /// </summary>
    Task<JobPost?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Retrieves all job posts created by a specific customer.
    /// </summary>
    Task<IEnumerable<JobPost>> GetJobsByCustomerIdAsync(Guid customerId, CancellationToken ct);

    /// <summary>
    /// Retrieves a paged and filtered list of job posts.
    /// </summary>
    Task<(IEnumerable<JobPost> Items, int TotalCount)> GetPagedJobsAsync(JobPostQuery query, CancellationToken ct);

    /// <summary>
    /// Retrieves job posts associated with a specific user, with an option to filter for only active jobs.
    /// </summary>
    Task<IEnumerable<JobPost>> GetJobsByUserIdAsync(Guid userId, bool onlyActive, CancellationToken ct);

    /// <summary>
    /// Retrieves the owner (customer) details for a specific job post.
    /// </summary>
    Task<AppUser?> GetCustomerByJobIdAsync(Guid jobId, CancellationToken ct);
}
