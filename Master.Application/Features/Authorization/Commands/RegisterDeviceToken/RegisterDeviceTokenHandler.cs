using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Authorization.Commands.RegisterDeviceToken;

/// <summary>
/// Command to register or update a mobile device push notification token.
/// </summary>
public record RegisterDeviceTokenCommand(Guid UserId, RegisterDeviceTokenRequest Request) : IRequest<bool>;

/// <summary>
/// Handler for updating user's mobile device token.
/// </summary>
public class RegisterDeviceTokenHandler : IRequestHandler<RegisterDeviceTokenCommand, bool>
{
    private readonly IAuthRepository _authRepository;

    public RegisterDeviceTokenHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<bool> Handle(RegisterDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        user.DeviceToken = request.Request.DeviceToken;
        user.DeviceType = request.Request.DeviceType ?? "mobile";
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var result = await _authRepository.UpdateAsync(user);
        return result.Succeeded;
    }
}
