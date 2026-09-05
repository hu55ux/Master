namespace Master.Application.DTOs;

/// <summary>
/// DTO for creating a new job offer / proposal for a job post.
/// </summary>
public class CreateJobOfferDTO
{
    public Guid JobPostId { get; set; }
    public decimal OfferedPrice { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset ScheduledStartDate { get; set; }
    public DateTimeOffset ScheduledEndDate { get; set; }
}

/// <summary>
/// Response DTO containing detailed information about a job offer.
/// </summary>
public class JobOfferResponseDTO
{
    public Guid Id { get; set; }
    public Guid JobPostId { get; set; }
    public string JobPostTitle { get; set; } = string.Empty;
    public Guid MasterId { get; set; }
    public string MasterFirstName { get; set; } = string.Empty;
    public string MasterLastName { get; set; } = string.Empty;
    public string? MasterProfileImageUrl { get; set; }
    public decimal MasterRating { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerFirstName { get; set; } = string.Empty;
    public string CustomerLastName { get; set; } = string.Empty;
    public decimal OfferedPrice { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset ScheduledStartDate { get; set; }
    public DateTimeOffset ScheduledEndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
