using FluentValidation;
using Master.DTOs;

namespace Master.Validators
{
    /// <summary>
    /// Validator for <see cref="CreateSkillDTO"/>.
    /// Ensures the skill's name and description meet minimum requirements.
    /// </summary>
    public class CreateSkillValidator : AbstractValidator<CreateSkillDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSkillValidator"/> class.
        /// </summary>
        public CreateSkillValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Skill name is required.")
                .MinimumLength(2).WithMessage("Skill name must be at least 2 characters long.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Skill description is required.")
                .MinimumLength(10).WithMessage("Skill description must be at least 10 characters long.");
        }
    }

    /// <summary>
    /// Validator for <see cref="UpdateSkillDTO"/>.
    /// Ensures the skill's name and description meet minimum requirements.
    /// </summary>
    public class UpdateSkillValidator : AbstractValidator<UpdateSkillDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSkillValidator"/> class.
        /// </summary>
        public UpdateSkillValidator()
        {
            // Reuse rules for consistency
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Skill name is required.")
                .MinimumLength(2).WithMessage("Skill name must be at least 2 characters long.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Skill description is required.")
                .MinimumLength(10).WithMessage("Skill description must be at least 10 characters long.");
        }
    }
}