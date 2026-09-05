using System.Security.Claims;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Features.Location.Commands.UpdateUserLocation;
using Master.Application.Features.Location.Queries.GetNavigationLinks;
using Master.Application.Features.Location.Queries.GetNearbyMasters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Master.API.Controllers;

/// <summary>
/// Systematic Controller for Hybrid Location management, Radius Filtering, Reverse Geocoding, and Navigation Deep Links.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User is not authenticated."));

    /// <summary>
    /// Updates the authenticated user's current GPS location and physical address.
    /// </summary>
    /// <param name="request">Latitude, Longitude coordinates and optional Address string.</param>
    [Authorize]
    [HttpPut("my-location")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateLocation([FromBody] UpdateUserLocationRequest request)
    {
        var result = await _mediator.Send(new UpdateUserLocationCommand(CurrentUserId, request.Latitude, request.Longitude, request.Address));
        return Ok(ApiResponse<bool>.SuccessResponse(result, "GPS location updated successfully."));
    }

    /// <summary>
    /// Finds nearby masters within a specified radius (in kilometers) from the provided coordinates.
    /// </summary>
    /// <param name="latitude">Target latitude coordinate.</param>
    /// <param name="longitude">Target longitude coordinate.</param>
    /// <param name="radiusKm">Search radius in kilometers (default: 5.0 km).</param>
    /// <returns>List of nearby masters sorted by distance with Waze and Google Maps navigation links.</returns>
    [HttpGet("nearby-masters")]
    [ProducesResponseType(typeof(ApiResponse<List<NearbyMasterDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<NearbyMasterDTO>>>> GetNearbyMasters(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 5.0)
    {
        var masters = await _mediator.Send(new GetNearbyMastersQuery(latitude, longitude, radiusKm));
        return Ok(ApiResponse<List<NearbyMasterDTO>>.SuccessResponse(masters, $"{masters.Count} master(s) found within {radiusKm} km radius."));
    }

    /// <summary>
    /// Generates Waze, Google Maps, and Apple Maps navigation deep links for a given set of coordinates.
    /// </summary>
    /// <param name="latitude">Target latitude coordinate.</param>
    /// <param name="longitude">Target longitude coordinate.</param>
    /// <param name="address">Optional custom address label.</param>
    /// <returns>Navigation links DTO containing Waze, Google Maps, and Apple Maps URLs.</returns>
    [HttpGet("navigation-links")]
    [ProducesResponseType(typeof(ApiResponse<NavigationLinksDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NavigationLinksDTO>>> GetNavigationLinks(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] string? address = null)
    {
        var result = await _mediator.Send(new GetNavigationLinksQuery(latitude, longitude, address));
        return Ok(ApiResponse<NavigationLinksDTO>.SuccessResponse(result, "Navigation deep links generated successfully."));
    }
}
