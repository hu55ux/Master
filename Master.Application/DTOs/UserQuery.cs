namespace Master.Application.DTOs;

/// <summary>
/// Represents the query parameters for filtering and paginating users.
/// </summary>
public class UserQuery
{
    /// <summary>
    /// Gets or sets the search term for filtering users.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets or sets the page number for pagination. Defaults to 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size for pagination. Defaults to 10.
    /// </summary>
    public int PageSize { get; set; } = 10;
}
