using Microsoft.AspNetCore.Identity;

namespace Master.Models;

/// <summary>
/// Represents an application user in the system.
/// </summary>
/// <remarks>
/// This class extends <see cref="IdentityUser{TKey}"/> and includes additional
/// profile information such as personal details, experience, and relationships
/// with skills and job posts.
/// </remarks>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    /// <example>Smith</example>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's professional experience in years.
    /// </summary>
    /// <remarks>
    /// Represents the number of years the user has worked in their profession.
    /// </remarks>
    public short Experience { get; set; } = 0;

    /// <summary>
    /// Gets or sets the user's age.
    /// </summary>
    public short Age { get; set; }

    /// <summary>
    /// Gets or sets the user's date of birth.
    /// </summary>
    public DateTimeOffset DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the date when the user account was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the date when the user profile was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property representing the skills owned by the user.
    /// </summary>
    /// <remarks>
    /// This is part of the many-to-many relationship between <see cref="AppUser"/> and <see cref="Skill"/>.
    /// </remarks>
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

    /// <summary>
    /// Navigation property representing job posts created by the user.
    /// </summary>
    /// <remarks>
    /// These are the job requests posted by the user acting as a customer.
    /// </remarks>
    public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}