using FluentValidation;
using Master.Application.DTOs;
using Master.Domain.Enums;

namespace Master.Application.Validators;

/// <summary>
/// Validator for UpdateMasterStatusRequest to ensure valid status ID or Name is provided.
/// </summary>
public class UpdateMasterStatusRequestValidator : AbstractValidator<UpdateMasterStatusRequest>
{
    public UpdateMasterStatusRequestValidator()
    {
        RuleFor(x => x)
            .Must(req => 
                (req.StatusId >= 1 && req.StatusId <= 3) || 
                (!string.IsNullOrWhiteSpace(req.StatusName) && MasterStatus.TryFromName(req.StatusName) != null))
            .WithMessage("Invalid master status value. Valid values are 1 (Available), 2 (Busy), 3 (Offline) or names 'Available', 'Busy', 'Offline'.");
    }
}
