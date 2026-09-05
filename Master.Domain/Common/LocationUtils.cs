namespace Master.Domain.Common;

/// <summary>
/// Utility methods for calculating geographic distances using the Haversine formula.
/// </summary>
public static class LocationUtils
{
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Calculates the great-circle distance between two points on the Earth in kilometers.
    /// </summary>
    /// <param name="lat1">Latitude of origin point.</param>
    /// <param name="lon1">Longitude of origin point.</param>
    /// <param name="lat2">Latitude of destination point.</param>
    /// <param name="lon2">Longitude of destination point.</param>
    /// <returns>Distance in kilometers (rounded to 2 decimal places).</returns>
    public static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadiusKm * c;

        return Math.Round(distance, 2);
    }

    private static double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180.0);
    }
}
