using Master.Application.DTOs;
using Master.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Master.Application.Interfaces;

public interface IAuthRepository
{
    Task<AppUser?> GetByIdAsync(string id);
    Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IdentityResult> UpdateAsync(AppUser user);
    Task<bool> CheckPasswordAsync(AppUser user, string password);
    Task<AppUser?> GetUserWithDetailsAsync(Guid userId, CancellationToken ct);
    Task<IdentityResult> FullDeleteUserAsync(AppUser user, CancellationToken ct);
    Task<RefreshToken?> GetRefreshTokenByJtiAsync(string jti);
    void UpdateRefreshToken(RefreshToken token);
    Task<IdentityResult> CreateUserAsync(AppUser user, string password);
    Task<bool> RoleExistsAsync(string roleName);
    Task CreateRoleAsync(string roleName);
    Task AddToRoleAsync(AppUser user, string roleName);
    Task SaveChangesAsync(CancellationToken ct);
}
