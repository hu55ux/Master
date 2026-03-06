using FluentValidation;
using Master.DTOs;
using Master.Models;

namespace Master.Validators;

public class CreateSkillValidator : AbstractValidator<CreateSkillDTO>
{
    public CreateSkillValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill name is required")
            .MinimumLength(2).WithMessage("Skill name must be at least 2 characters long");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Skill description is required")
            .MinimumLength(10).WithMessage("Skill description must be at least 10 characters long");
    }
}
public class UpdateSkillValidator : AbstractValidator<UpdateSkillDTO>
{
    public UpdateSkillValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill name is required")
            .MinimumLength(2).WithMessage("Skill name must be at least 2 characters long");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Skill description is required")
            .MinimumLength(10).WithMessage("Skill description must be at least 10 characters long");
    }
}
