using Master.Domain.Enums;

namespace Master.Application.DTOs;

/// <summary>
/// DTO for requesting an update to a master's availability status.
/// Accepts status by ID (1 = Available, 2 = Busy, 3 = Offline) or by Name ("Available", "Busy", "Offline").
/// </summary>
public class UpdateMasterStatusRequest
{
    /// <summary>
    /// The new availability status identifier (1 = Available, 2 = Busy, 3 = Offline).
    /// </summary>
    public int StatusId { get; set; } = 1;

    /// <summary>
    /// Optional status name string ("Available", "Busy", "Offline").
    /// </summary>
    public string? StatusName { get; set; }
}

/// <summary>
/// Data Transfer Object representing a master's current work availability status with rich metadata.
/// </summary>
public class MasterStatusResponseDTO
{
    /// <summary>
    /// Gets or sets the unique identifier of the master.
    /// </summary>
    public Guid MasterId { get; set; }

    /// <summary>
    /// Gets or sets the full name of the master.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the integer ID of the status.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Gets or sets the code name of the status (e.g. "Available", "Busy", "Offline").
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the localized display name of the status (e.g. "Hazır", "Məşğul", "Qeyri-aktiv").
    /// </summary>
    public string StatusDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status badge color code for UI rendering.
    /// </summary>
    public string ColorCode { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the master can accept new job requests in this status.
    /// </summary>
    public bool CanAcceptJobs { get; set; }

    /// <summary>
    /// Gets or sets the date/time when the status was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}

/// <summary>
/// Lookup DTO for returning available MasterStatus options (e.g. for frontend dropdowns).
/// </summary>
public record MasterStatusLookupDto(int Id, string Name, string DisplayName, string ColorCode, bool CanAcceptJobs);
