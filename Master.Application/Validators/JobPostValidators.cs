using FluentValidation;
using Master.Application.DTOs;

namespace Master.Application.Validators
{
    /// <summary>
    /// Validator for <see cref="CreateJobPostDTO"/>.
    /// Ensures all required fields for creating a job post are valid.
    /// </summary>
    public class CreateJPValidator : AbstractValidator<CreateJobPostDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJPValidator"/> class.
        /// </summary>
        public CreateJPValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Budget)
                .GreaterThan(0).WithMessage("Budget must be greater than zero.");

            RuleFor(x => x.RequiredSkillId)
                .NotEmpty().WithMessage("Required skill is mandatory.");
        }
    }

    /// <summary>
    /// Validator for <see cref="UpdateJobPostDTO"/>.
    /// Validates optional fields during a job post update.
    /// </summary>
    public class UpdateJPValidator : AbstractValidator<UpdateJobPostDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateJPValidator"/> class.
        /// </summary>
        public UpdateJPValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().When(x => x.Title != null)
                .MaximumLength(100).When(x => x.Title != null)
                .WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().When(x => x.Description != null)
                .MaximumLength(1000).When(x => x.Description != null)
                .WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Budget)
                .GreaterThan(0).When(x => x.Budget.HasValue)
                .WithMessage("Budget must be greater than zero.");

            RuleFor(x => x.RequiredSkillId)
                .NotEmpty().When(x => x.RequiredSkillId.HasValue)
                .WithMessage("Required skill ID cannot be empty.");
        }
    }
}