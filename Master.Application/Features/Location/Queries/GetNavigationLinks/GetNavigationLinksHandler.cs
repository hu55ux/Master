using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Location.Queries.GetNavigationLinks;

/// <summary>
/// Query to generate navigation deep links (Waze, Google Maps, Apple Maps) for a specific set of coordinates.
/// </summary>
public record GetNavigationLinksQuery(double Latitude, double Longitude, string? Address = null) : IRequest<NavigationLinksDTO>;

public class GetNavigationLinksHandler : IRequestHandler<GetNavigationLinksQuery, NavigationLinksDTO>
{
    private readonly ILocationService _locationService;

    public GetNavigationLinksHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<NavigationLinksDTO> Handle(GetNavigationLinksQuery request, CancellationToken cancellationToken)
    {
        var address = request.Address;
        if (string.IsNullOrWhiteSpace(address))
        {
            address = await _locationService.ReverseGeocodeAsync(request.Latitude, request.Longitude, cancellationToken);
        }

        return _locationService.GenerateNavigationLinks(request.Latitude, request.Longitude, address);
    }
}
