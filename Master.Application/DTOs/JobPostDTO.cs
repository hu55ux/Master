using Master.Domain.Models;

namespace Master.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new job post.
    /// </summary>
    public class CreateJobPostDTO
    {
        /// <summary>
        /// Gets or sets the title of the job post.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the job post.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location of the job.
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the budget for the job post.
        /// </summary>
        public decimal Budget { get; set; }

        /// <summary>
        /// Gets or sets the ID of the required skill for this job post.
        /// </summary>
        public Guid RequiredSkillId { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for updating an existing job post.
    /// Only provided fields will be updated.
    /// </summary>
    public class UpdateJobPostDTO
    {
        /// <summary>
        /// Gets or sets the title of the job post.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the job post.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the location of the job run.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the budget for the job post.
        /// </summary>
        public decimal? Budget { get; set; }

        /// <summary>
        /// Gets or sets the ID of the required skill for this job post.
        /// </summary>
        public Guid? RequiredSkillId { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for returning job post information to clients.
    /// </summary>
    public class JobPostResponseDTO
    {
        /// <summary>
        /// Gets or sets the ID of the job post.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the job post.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the job post.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location of the job.
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the budget for the job post.
        /// </summary>
        public decimal? Budget { get; set; }

        /// <summary>
        /// Gets or sets the status of the job post.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the creation date of the job post.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the customer who created the job post.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the customer.
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the required skill for this job post.
        /// </summary>
        public Guid RequiredSkillId { get; set; }

        /// <summary>
        /// Gets or sets the name of the required skill.
        /// </summary>
        public string RequiredSkillName { get; set; } = string.Empty;

        /// <summary>
        /// Collection of Cloudinary CDN image URLs attached to this job post.
        /// </summary>
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}

/// <summary>
/// Represents the query parameters for filtering and paginating job posts.
/// </summary>
public class JobPostQuery
{
    /// <summary>
    /// Page number for pagination. Defaults to 1 if not provided.
    /// </summary>
    public int Page { get; set; }
    /// <summary>
    /// Page size for pagination. Defaults to 10 if not provided.
    /// Maximum allowed is 100 to prevent performance issues.
    /// </summary>
    public int PageSize { get; set; }
    /// <summary>
    /// Search term for filtering invoices by customer name, 
    /// invoice comment, or other relevant fields.
    /// </summary>
    public string? SearchTerm { get; set; }
    /// <summary>
    /// Sorting field for ordering results. Common values include 
    /// "StartDate", "EndDate", "TotalSum", etc.
    /// </summary>
    public string Sort { get; set; } = string.Empty;
    /// <summary>
    /// Sorting direction for ordering results. Common values are "asc" for ascending
    /// desc for descending. Defaults to "asc" if not provided.
    /// </summary>
    public string SortDirection { get; set; } = string.Empty;
    /// <summary>
    /// Invoice status for filtering results. Common values include "Created", "Sent", "Paid", "Cancelled", etc.
    /// </summary>
    public JobPostStatus? Status { get; set; }

    public void Validate()
    {
        if (Page < 1) Page = 1;

        if (PageSize < 1) PageSize = 1;

        if (PageSize > 100) PageSize = 100;

        if (string.IsNullOrWhiteSpace(SortDirection)) SortDirection = "asc";

        SortDirection = SortDirection.ToLower();

        if (SortDirection != "asc" && SortDirection != "desc") SortDirection = "asc";
    }
}

/// <summary>
/// Job status lookup DTO for returning job status information to clients. This is used to provide a simple representation of job statuses, typically for dropdowns or selection lists in the UI.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
public record JobStatusLookupDto(int Id, string Name);

