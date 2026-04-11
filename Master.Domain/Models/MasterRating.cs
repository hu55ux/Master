namespace Master.Domain.Models;

/// <summary>
/// Represents a rating given to a master by a customer.
/// </summary>
public class MasterRating
{
    /// <summary>
    /// Gets or sets the unique identifier of the user who is being rated (the master).
    /// </summary>
    public Guid MasterId { get; set; }
    /// <summary>
    /// Navigation property representing the user who is being rated.
    /// </summary>
    public virtual AppUser? Master { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier of the user who gave the rating (the customer).
    /// </summary>
    public Guid CustomerId { get; set; }
    /// <summary>
    /// Navigation property representing the user who gave the rating.
    /// </summary>
    public virtual AppUser? Customer { get; set; }
    /// <summary>
    /// Gets or sets the score given by the customer (e.g., from 1 to 5).
    /// </summary>
    public decimal Score { get; set; }
    /// <summary>
    /// Gets or sets an optional comment provided by the customer.
    /// </summary>
    public string? Comment { get; set; }
    /// <summary>
    /// Gets or sets the date and time when the rating was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}