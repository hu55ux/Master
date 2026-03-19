using Master.Application.Interfaces;
using Master.Application.Models;
using Master.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Master.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly MasterDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public AuthRepository(MasterDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
        => await _userManager.FindByEmailAsync(email);

    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Users.FindAsync(new object[] { id }, ct);

    public async Task<AppUser?> GetByIdAsync(string id)
         => await _userManager.FindByIdAsync(id);

    public async Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
        => await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

    public async Task<AppUser?> GetUserWithDetailsAsync(Guid userId, CancellationToken ct)
    {
        return await _context.Users
         .Include(u => u.UserSkills).ThenInclude(us => us.Skill)
         .Include(u => u.JobPosts)
         .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task<IdentityResult> FullDeleteUserAsync(AppUser user, CancellationToken ct)
    {
        if (user.UserSkills.Any()) _context.UserSkills.RemoveRange(user.UserSkills);
        if (user.JobPosts.Any()) _context.JobPosts.RemoveRange(user.JobPosts);

        var tokens = _context.RefreshTokens.Where(t => t.UserId == user.Id.ToString());
        _context.RefreshTokens.RemoveRange(tokens);

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            await _context.SaveChangesAsync(ct);
        }

        return result;
    }

    public async Task<IdentityResult> UpdateAsync(AppUser user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<bool> CheckPasswordAsync(AppUser user, string password)
    => await _userManager.CheckPasswordAsync(user, password);

    public async Task<RefreshToken?> GetRefreshTokenByJtiAsync(string jti)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);
    }

    public void UpdateRefreshToken(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IdentityResult> CreateUserAsync(AppUser user, string password)
        => await _userManager.CreateAsync(user, password);

    public async Task<bool> RoleExistsAsync(string roleName)
        => await _roleManager.RoleExistsAsync(roleName);

    public async Task CreateRoleAsync(string roleName)
        => await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

    public async Task AddToRoleAsync(AppUser user, string roleName)
        => await _userManager.AddToRoleAsync(user, roleName);
}
