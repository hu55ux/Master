namespace Master.Application.Models;

/// <summary>
/// Represents a skill that can be associated with users and job posts.
/// </summary>
/// <remarks>
/// A skill defines a specific ability or expertise. 
/// Users can have multiple skills and job posts may require a specific skill.
/// </remarks>
public class Skill
{
    /// <summary>
    /// Gets or sets the unique identifier of the skill.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the skill.
    /// </summary>
    /// <example>Plumbing, Electrical Repair, Painting</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the detailed description of the skill.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Navigation property representing the users who have this skill.
    /// </summary>
    /// <remarks>
    /// This is part of the many-to-many relationship between <see cref="AppUser"/> and <see cref="Skill"/>.
    /// </remarks>
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

    /// <summary>
    /// Navigation property representing job posts that require this skill.
    /// </summary>
    public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}