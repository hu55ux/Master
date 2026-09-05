using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Master.Application.DTOs;
using Master.Application.Interfaces;

namespace Master.Infrastructure.Services;

/// <summary>
/// Infrastructure service implementing free hybrid location capabilities:
/// - Navigation deep link generation for Waze, Google Maps, Apple Maps
/// - OpenStreetMap (Nominatim) free reverse geocoding
/// </summary>
public class LocationService : ILocationService
{
    private readonly HttpClient _httpClient;

    public LocationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MasterApp/1.0 (contact@master.app)");
    }

    public NavigationLinksDTO GenerateNavigationLinks(double latitude, double longitude, string? address = null)
    {
        var latStr = latitude.ToString(CultureInfo.InvariantCulture);
        var lonStr = longitude.ToString(CultureInfo.InvariantCulture);

        return new NavigationLinksDTO
        {
            Latitude = latitude,
            Longitude = longitude,
            FormattedAddress = address,
            WazeUrl = $"https://waze.com/ul?ll={latStr},{lonStr}&navigate=yes",
            GoogleMapsUrl = $"https://www.google.com/maps/dir/?api=1&destination={latStr},{lonStr}",
            AppleMapsUrl = $"https://maps.apple.com/?daddr={latStr},{lonStr}"
        };
    }

    public async Task<string> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            var latStr = latitude.ToString(CultureInfo.InvariantCulture);
            var lonStr = longitude.ToString(CultureInfo.InvariantCulture);
            var url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={latStr}&lon={lonStr}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                using var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
                if (jsonDoc.RootElement.TryGetProperty("display_name", out var displayName))
                {
                    return displayName.GetString() ?? $"{latitude}, {longitude}";
                }
            }
        }
        catch
        {
            // Fallback gracefully to coordinate string if network/rate-limited
        }

        return $"{latitude}, {longitude}";
    }
}
