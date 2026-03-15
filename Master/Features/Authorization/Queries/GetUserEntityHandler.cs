using Master.Data;
using Master.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Master.Features.Authorization.Queries;

public class GetUserEntityHandler : IRequestHandler<GetUserEntityQuery, AppUser>
{
    private readonly MasterDbContext _context;

    public GetUserEntityHandler(MasterDbContext context) => _context = context;

    public async Task<AppUser> Handle(GetUserEntityQuery query, CancellationToken ct)
    {
        var user = await _context.Users
            .Include(u => u.UserSkills).ThenInclude(us => us.Skill)
            .Include(u => u.JobPosts)
            .FirstOrDefaultAsync(u => u.Id == query.UserId);

        return user ?? throw new KeyNotFoundException("User not found.");
    }
}
