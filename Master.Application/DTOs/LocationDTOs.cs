namespace Master.Application.DTOs;

/// <summary>
/// Request DTO for updating authenticated user's GPS coordinates and location.
/// </summary>
public class UpdateUserLocationRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
}

/// <summary>
/// Response DTO containing navigation deep links for Waze, Google Maps, Apple Maps.
/// </summary>
public class NavigationLinksDTO
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? FormattedAddress { get; set; }
    public string WazeUrl { get; set; } = string.Empty;
    public string GoogleMapsUrl { get; set; } = string.Empty;
    public string AppleMapsUrl { get; set; } = string.Empty;
}

/// <summary>
/// DTO representing a nearby master found within a search radius.
/// </summary>
public class NearbyMasterDTO
{
    public Guid MasterId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public decimal AverageRating { get; set; }
    public string Status { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Calculated distance from user in kilometers.
    /// </summary>
    public double DistanceKm { get; set; }

    /// <summary>
    /// Deep link to launch Waze navigation to this master's location.
    /// </summary>
    public string WazeUrl { get; set; } = string.Empty;

    /// <summary>
    /// Deep link to launch Google Maps navigation to this master's location.
    /// </summary>
    public string GoogleMapsUrl { get; set; } = string.Empty;

    public List<string> Skills { get; set; } = new List<string>();
}
