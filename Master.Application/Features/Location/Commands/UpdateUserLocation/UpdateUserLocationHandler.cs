using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Location.Commands.UpdateUserLocation;

public record UpdateUserLocationCommand(Guid UserId, double Latitude, double Longitude, string? Address) : IRequest<bool>;

public class UpdateUserLocationHandler : IRequestHandler<UpdateUserLocationCommand, bool>
{
    private readonly IAuthRepository _authRepository;
    private readonly ILocationService _locationService;

    public UpdateUserLocationHandler(IAuthRepository authRepository, ILocationService locationService)
    {
        _authRepository = authRepository;
        _locationService = locationService;
    }

    public async Task<bool> Handle(UpdateUserLocationCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        user.Latitude = request.Latitude;
        user.Longitude = request.Longitude;

        if (!string.IsNullOrWhiteSpace(request.Address))
        {
            user.Address = request.Address;
        }
        else
        {
            // Reverse geocode via OpenStreetMap if no address provided
            user.Address = await _locationService.ReverseGeocodeAsync(request.Latitude, request.Longitude, cancellationToken);
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _authRepository.UpdateAsync(user);
        return true;
    }
}
