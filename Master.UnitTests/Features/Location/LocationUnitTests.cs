using FluentAssertions;
using Master.Domain.Common;
using Master.Infrastructure.Services;
using System.Net.Http;
using Xunit;

namespace Master.UnitTests.Features.Location;

public class LocationUnitTests
{
    [Fact]
    public void Haversine_Distance_Calculation_Should_Be_Accurate()
    {
        // Baku 28 May area (40.3793, 49.8471) to Baku Port Mall (40.3756, 49.8617) ~1.3 km
        double distance = LocationUtils.CalculateDistanceKm(40.3793, 49.8471, 40.3756, 49.8617);

        distance.Should().BeGreaterThan(0.5);
        distance.Should().BeLessThan(3.0);
    }

    [Fact]
    public void GenerateNavigationLinks_Should_Return_Valid_Waze_And_Google_Urls()
    {
        var service = new LocationService(new HttpClient());
        var links = service.GenerateNavigationLinks(40.4093, 49.8671, "Baku Center");

        links.WazeUrl.Should().Contain("waze.com/ul?ll=40.4093,49.8671");
        links.GoogleMapsUrl.Should().Contain("google.com/maps/dir/?api=1&destination=40.4093,49.8671");
        links.AppleMapsUrl.Should().Contain("maps.apple.com/?daddr=40.4093,49.8671");
    }
}
