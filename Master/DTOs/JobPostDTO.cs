namespace Master.DTOs
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
    }
}