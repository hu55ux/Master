namespace Master.Models;

/// <summary>
/// Represents a job post created by a customer requesting a specific service.
/// </summary>
/// <remarks>
/// A job post describes a task or service request that can be viewed by users with the required skill.
/// Each job post is associated with a customer and a required skill.
/// </remarks>
public class JobPost
{
    /// <summary>
    /// Gets or sets the unique identifier of the job post.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the title of the job post.
    /// </summary>
    /// <example>Fix kitchen sink leak</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of the job.
    /// </summary>
    /// <example>The kitchen sink pipe is leaking and needs repair.</example>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the budget allocated for the job.
    /// </summary>
    /// <remarks>
    /// This value may be null if the customer did not specify a budget.
    /// </remarks>
    public decimal? Budget { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the job post was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the current status of the job post.
    /// </summary>
    public JobPostStatus JPStatus { get; set; } = JobPostStatus.Pending;

    /// <summary>
    /// Gets or sets the identifier of the customer who created the job post.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Navigation property representing the customer who created the job post.
    /// </summary>
    public AppUser? Customer { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the skill required for the job.
    /// </summary>
    public Guid RequiredSkillId { get; set; }

    /// <summary>
    /// Navigation property representing the skill required to perform the job.
    /// </summary>
    public Skill? RequiredSkill { get; set; }
}

/// <summary>
/// Represents the possible states of a job post during its lifecycle.
/// </summary>
public enum JobPostStatus
{
    /// <summary>
    /// The job post is created but not yet approved or activated.
    /// </summary>
    Pending,

    /// <summary>
    /// The job post is active and visible to workers.
    /// </summary>
    Active,

    /// <summary>
    /// The job has been accepted by a worker and work is currently in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// The job has been completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The job has been canceled by the customer or system.
    /// </summary>
    Canceled
}