namespace Master.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new skill.
    /// </summary>
    public class CreateSkillDTO
    {
        /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description of the skill.
        /// </summary>
        public string Description { get; set; } = null!;
    }

    /// <summary>
    /// Data Transfer Object for updating an existing skill.
    /// </summary>
    public class UpdateSkillDTO
    {
        /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description of the skill.
        /// </summary>
        public string Description { get; set; } = null!;
    }

    /// <summary>
    /// Data Transfer Object for returning skill information to clients.
    /// </summary>
    public class SkillResponseDTO
    {
        /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the skill.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}

public class SkillQuery
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