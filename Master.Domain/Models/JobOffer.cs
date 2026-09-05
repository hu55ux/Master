namespace Master.Domain.Models;

/// <summary>
/// Represents a job offer/proposal submitted by a master for a specific customer job post.
/// </summary>
public class JobOffer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the target job post.
    /// </summary>
    public Guid JobPostId { get; set; }
    public virtual JobPost? JobPost { get; set; }

    /// <summary>
    /// Identifier of the master submitting the proposal.
    /// </summary>
    public Guid MasterId { get; set; }
    public virtual AppUser? Master { get; set; }

    /// <summary>
    /// Identifier of the customer who owns the job post.
    /// </summary>
    public Guid CustomerId { get; set; }
    public virtual AppUser? Customer { get; set; }

    /// <summary>
    /// Proposed price in local currency (AZN).
    /// </summary>
    public decimal OfferedPrice { get; set; }

    /// <summary>
    /// Master's proposal message or work description.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Proposed or scheduled work start date and time.
    /// </summary>
    public DateTimeOffset ScheduledStartDate { get; set; }

    /// <summary>
    /// Proposed or scheduled work completion date and time.
    /// </summary>
    public DateTimeOffset ScheduledEndDate { get; set; }

    /// <summary>
    /// Current lifecycle status of the job offer.
    /// </summary>
    public JobOfferStatus Status { get; set; } = JobOfferStatus.Pending;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}

/// <summary>
/// Status enumeration for Job Offers.
/// </summary>
public enum JobOfferStatus
{
    /// <summary>
    /// Offer is submitted and awaiting customer decision.
    /// </summary>
    Pending,

    /// <summary>
    /// Offer has been accepted by the customer. Master becomes Busy.
    /// </summary>
    Accepted,

    /// <summary>
    /// Offer has been rejected by the customer.
    /// </summary>
    Rejected,

    /// <summary>
    /// Work has been completed successfully. Master becomes Available again.
    /// </summary>
    Completed,

    /// <summary>
    /// Offer has been canceled.
    /// </summary>
    Canceled
}
