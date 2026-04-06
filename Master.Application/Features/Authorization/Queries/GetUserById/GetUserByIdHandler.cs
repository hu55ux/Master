using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Master.Application.Features.Authorization.Queries.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, AuthResponseDTO>
{
    private readonly IAuthRepository _authRepository;
    private readonly UserManager<AppUser> _userManager;

    public GetUserByIdHandler(IAuthRepository authRepository, UserManager<AppUser> userManager)
    {
        _authRepository = authRepository;
        _userManager = userManager;
    }

    public async Task<AuthResponseDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetByIdAsync(request.id, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException("İstifadəçi tapılmadı.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponseDTO
        {
            Id = user.Id,
            Email = user.Email,
            AccessToken = null, // Access token is not generated in this query, it can be generated in a separate authentication process
            RefreshToken = null, // Refresh token is not generated in this query, it can be generated in a separate authentication process
            RefreshTokenExpiresAt = DateTimeOffset.UtcNow, // Set to current time, can be updated when generating refresh token
            FirstName = user.FirstName,
            LastName = user.LastName,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            Experience = user.Experience,
            DateOfBirth = user.DateOfBirth,
            AverageScore = user.AverageRating,
            Roles = roles.ToList()
        };
    }
}