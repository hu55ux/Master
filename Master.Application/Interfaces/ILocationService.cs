using Master.Application.DTOs;

namespace Master.Application.Interfaces;

/// <summary>
/// Service interface for location helper functions, navigation deep link generation, and reverse geocoding.
/// </summary>
public interface ILocationService
{
    NavigationLinksDTO GenerateNavigationLinks(double latitude, double longitude, string? address = null);
    Task<string> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}
