using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Application.Features.Location.Queries.GetNearbyMasters;

/// <summary>
/// Query to retrieve nearby masters within a specified radius (in kilometers) using Haversine calculation.
/// </summary>
public record GetNearbyMastersQuery(double Latitude, double Longitude, double RadiusKm = 5.0, Guid? SkillId = null) : IRequest<List<NearbyMasterDTO>>;

public class GetNearbyMastersHandler : IRequestHandler<GetNearbyMastersQuery, List<NearbyMasterDTO>>
{
    private readonly IAuthRepository _authRepository;
    private readonly ILocationService _locationService;

    public GetNearbyMastersHandler(IAuthRepository authRepository, ILocationService locationService)
    {
        _authRepository = authRepository;
        _locationService = locationService;
    }

    public async Task<List<NearbyMasterDTO>> Handle(GetNearbyMastersQuery request, CancellationToken cancellationToken)
    {
        var radius = request.RadiusKm <= 0 ? 5.0 : request.RadiusKm;

        // Fetch users who have valid GPS coordinates
        var users = await _authRepository.GetAllUsersAsync(cancellationToken);
        var masters = users
            .Where(u => u.Latitude.HasValue && u.Longitude.HasValue)
            .ToList();

        var result = new List<NearbyMasterDTO>();

        foreach (var master in masters)
        {
            var distance = LocationUtils.CalculateDistanceKm(
                request.Latitude, 
                request.Longitude, 
                master.Latitude!.Value, 
                master.Longitude!.Value);

            if (distance <= radius)
            {
                var links = _locationService.GenerateNavigationLinks(master.Latitude.Value, master.Longitude.Value, master.Address);

                result.Add(new NearbyMasterDTO
                {
                    MasterId = master.Id,
                    FirstName = master.FirstName,
                    LastName = master.LastName,
                    Email = master.Email ?? string.Empty,
                    ProfileImageUrl = master.ProfileImageUrl,
                    AverageRating = master.AverageRating,
                    Status = master.Status.ToString(),
                    Latitude = master.Latitude.Value,
                    Longitude = master.Longitude.Value,
                    Address = master.Address,
                    DistanceKm = distance,
                    WazeUrl = links.WazeUrl,
                    GoogleMapsUrl = links.GoogleMapsUrl,
                    Skills = master.UserSkills?.Select(us => us.Skill?.Name ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>()
                });
            }
        }

        // Return masters ordered by nearest distance first
        return result.OrderBy(m => m.DistanceKm).ToList();
    }
}
