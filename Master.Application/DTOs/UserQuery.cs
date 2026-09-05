using Master.Domain.Enums;

namespace Master.Application.DTOs;

/// <summary>
/// Represents the query parameters for filtering and paginating users.
/// </summary>
public class UserQuery
{
    /// <summary>
    /// Gets or sets the search term for filtering users by name.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets or sets the master status filter (e.g. "Available", "Busy", "Offline" or 1, 2, 3).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the page number for pagination. Defaults to 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size for pagination. Defaults to 10.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Resolves the MasterStatus Smart Enum instance from the provided Status query string.
    /// </summary>
    public MasterStatus? GetMasterStatus()
    {
        if (string.IsNullOrWhiteSpace(Status)) return null;
        if (int.TryParse(Status, out var id)) return MasterStatus.TryFromId(id);
        return MasterStatus.TryFromName(Status);
    }
}
