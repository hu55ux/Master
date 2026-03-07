using Microsoft.AspNetCore.Identity;

namespace Master.Models;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public short Experience { get; set; } = 0;
    public short Age { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; } = null!;
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}
