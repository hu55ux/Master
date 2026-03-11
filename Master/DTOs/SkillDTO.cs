namespace Master.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new skill.
    /// </summary>
    public class CreateSkillDTO
    {
        /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description of the skill.
        /// </summary>
        public string Description { get; set; } = null!;
    }

    /// <summary>
    /// Data Transfer Object for updating an existing skill.
    /// </summary>
    public class UpdateSkillDTO
    {
        /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description of the skill.
        /// </summary>
        public string Description { get; set; } = null!;
    }

    /// <summary>
    /// Data Transfer Object for returning skill information to clients.
    /// </summary>
    public class SkillResponseDTO
    {
        /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the skill.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}