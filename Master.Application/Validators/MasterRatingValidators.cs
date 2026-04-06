using FluentValidation;
using Master.Application.DTOs;

namespace Master.Application.Validators;

/// <summary>
/// Validator for creating a new master rating, ensuring all necessary fields are valid.
/// </summary>
public class CreateMasterRatingValidator : AbstractValidator<CreateMasterRatingDTO>
{
    public CreateMasterRatingValidator()
    {
        RuleFor(x => x.MasterId)
            .NotEmpty().WithMessage("MasterId is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.")
            .NotEqual(x => x.MasterId).WithMessage("A master cannot rate themselves.");

        RuleFor(x => x.Score)
            .InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
    }
}

/// <summary>
/// Validator for updating an existing master rating, ensuring constraints are preserved.
/// </summary>
public class UpdateMasterRatingValidator : AbstractValidator<UpdateMasterRatingDTO>
{
    public UpdateMasterRatingValidator()
    {
        RuleFor(x => x.MasterId)
            .NotEmpty().WithMessage("MasterId is required.");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.")
            .NotEqual(x => x.MasterId).WithMessage("A master cannot rate themselves.");

        RuleFor(x => x.Score)
            .InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
    }
}
