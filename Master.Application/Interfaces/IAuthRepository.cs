using Master.Application.Common;
using Master.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Master.Application.Interfaces;

/// <summary>
/// Interface for authentication and user management repository operations.
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Retrieves a user by their unique string identity.
    /// </summary>
    Task<AppUser?> GetByIdAsync(string id);

    /// <summary>
    /// Changes the password for a specific user.
    /// </summary>
    Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    Task<AppUser?> GetByEmailAsync(string email);

    /// <summary>
    /// Retrieves a user by their unique Guid identity.
    /// </summary>
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Updates the user's profile information.
    /// </summary>
    Task<IdentityResult> UpdateAsync(AppUser user);

    /// <summary>
    /// Checks if the provided password is valid for the given user.
    /// </summary>
    Task<bool> CheckPasswordAsync(AppUser user, string password);

    /// <summary>
    /// Retrieves a user with their associated details (e.g., skills, ratings).
    /// </summary>
    Task<AppUser?> GetUserWithDetailsAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Permanently deletes a user and all their associated data.
    /// </summary>
    Task<IdentityResult> FullDeleteUserAsync(AppUser user, CancellationToken ct);

    /// <summary>
    /// Retrieves a refresh token by its JWT identifier (JTI).
    /// </summary>
    Task<RefreshToken?> GetRefreshTokenByJtiAsync(string jti);

    /// <summary>
    /// Updates an existing refresh token in the database.
    /// </summary>
    void UpdateRefreshToken(RefreshToken token);

    /// <summary>
    /// Creates a new user with the specified password.
    /// </summary>
    Task<IdentityResult> CreateUserAsync(AppUser user, string password);

    /// <summary>
    /// Checks if a role with the specified name exists.
    /// </summary>
    Task<bool> RoleExistsAsync(string roleName);

    /// <summary>
    /// Creates a new role in the system.
    /// </summary>
    Task CreateRoleAsync(string roleName);

    /// <summary>
    /// Assigns a role to a specific user.
    /// </summary>
    Task AddToRoleAsync(AppUser user, string roleName);

    /// <summary>
    /// Retrieves a paged list of users filtered by role, search term, status, and sorting.
    /// </summary>
    Task<PagedResult<AppUser>> GetUsersPagedAsync(string roleName, int pageNumber, int pageSize, string? search, string? orderBy, Master.Domain.Enums.MasterStatus? status = null);

    /// <summary>
    /// Retrieves all users with their skills for distance and location calculations.
    /// </summary>
    Task<List<AppUser>> GetAllUsersAsync(CancellationToken ct);

    /// <summary>
    /// Persists all changes to the database.
    /// </summary>
    Task SaveChangesAsync(CancellationToken ct);
}
