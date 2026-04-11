namespace Master.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new master rating.
/// </summary>
public class CreateMasterRatingDTO
{
    /// <summary>
    /// Gets or sets the ID of the master being rated.
    /// </summary>
    public Guid MasterId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the customer giving the rating.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the score of the rating.
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// Gets or sets an optional comment for the rating.
    /// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// Data Transfer Object for updating an existing master rating.
/// </summary>
public class UpdateMasterRatingDTO
{
    /// <summary>
    /// Gets or sets the ID of the master whose rating is being updated.
    /// </summary>
    public Guid MasterId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the customer who gave the rating.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the new score of the rating.
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// Gets or sets the new comment for the rating.
    /// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// Data Transfer Object for returning master rating information.
/// </summary>
public class MasterRatingResponseDTO
{
    /// <summary>
    /// Gets or sets the ID of the master.
    /// </summary>
    public Guid MasterId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the customer.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the name of the master.
    /// </summary>
    public string? MasterName { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer.
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the score of the rating.
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// Gets or sets the comment for the rating.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets the date when the rating was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
