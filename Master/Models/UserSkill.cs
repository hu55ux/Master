namespace Master.Models;

/// <summary>
/// Represents the relationship between a user and a skill.
/// This entity is used as a join table for the many-to-many relationship
/// between <see cref="AppUser"/> and <see cref="Skill"/>.
/// </summary>
public class UserSkill
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property representing the associated user.
    /// </summary>
    public AppUser? User { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the skill.
    /// </summary>
    public Guid SkillId { get; set; }

    /// <summary>
    /// Navigation property representing the associated skill.
    /// </summary>
    public Skill? Skill { get; set; }
}