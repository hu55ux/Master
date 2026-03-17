using Master.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Master.Application.Features.Authorization.Commands.DeleteProfile;

public class DeleteProfileHandler : IRequestHandler<DeleteProfileCommand, bool>
{
    private readonly MasterDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public DeleteProfileHandler(MasterDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> Handle(DeleteProfileCommand command, CancellationToken ct)
    {
        var user = await _context.Users
            .Include(u => u.UserSkills)
            .Include(u => u.JobPosts)
            .FirstOrDefaultAsync(u => u.Id == command.UserId, ct);

        if (user is null) throw new KeyNotFoundException("Profile not found.");

        if (user.UserSkills.Any()) _context.UserSkills.RemoveRange(user.UserSkills);
        if (user.JobPosts.Any()) _context.JobPosts.RemoveRange(user.JobPosts);

        var tokens = _context.RefreshTokens.Where(t => t.UserId == command.UserId.ToString());
        _context.RefreshTokens.RemoveRange(tokens);

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User deleting failed: {errors}");
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}
