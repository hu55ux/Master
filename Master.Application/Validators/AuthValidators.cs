using FluentValidation;
using Master.Application.DTOs;

namespace Master.Application.Validators;

/// <summary>
/// Validator for the login request to ensure required credentials are provided and properly formatted.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>
    /// Initializes validation rules for email format and password complexity during login.
    /// </summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email is required")
           .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Passwords must be at least 6 characters.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
                .WithMessage("Passwords must have at least one digit ('0'-'9'), " +
                "one lowercase ('a'-'z'), and one uppercase ('A'-'Z').");
    }
}

/// <summary>
/// Validator for the registration request to enforce data integrity for new user accounts.
/// </summary>
/// <summary>
/// Validator for the <see cref="RegisterRequest"/> model.
/// Ensures all required fields for user registration are provided and follow business rules.
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private readonly string[] _allowedRoles = { "Master", "Customer" };
    /// <summary>
    /// Constructor that sets up validation rules for user registration, including checks for name length, email format, password strength, and confirmation match.
    /// </summary>
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters long");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Phone number is not valid (must be 10-15 digits)");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Passwords must be at least 6 characters.")
            .Password().WithMessage("Passwords must have at least one digit ('0'-'9'), one lowercase ('a'-'z'), and one uppercase ('A'-'Z').");

        RuleFor(x => x.ConfirmedPassword)
            .NotEmpty().WithMessage("Confirmation password is required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(BeAtLeast18).WithMessage("You must be at least 18 years old.");

        RuleFor(x => x.Experience)
            .NotEmpty().WithMessage("Experience is required")
            .GreaterThanOrEqualTo((short)0).WithMessage("Experience must be a non-negative number")
            .LessThanOrEqualTo((short)40).WithMessage("Experience must be less than or equal to 40 years");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role field cannot be empty..")
            .Must(role => _allowedRoles.Contains(role))
            .WithMessage($"Invalid role selection. Allowed roles: {string.Join(", ", _allowedRoles)}");

    }

    /// <summary>
    /// Custom logic to check if the user is 18 years or older.
    /// </summary>
    private bool BeAtLeast18(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
            age--;

        return age >= 18;
    }
}

/// <summary>
/// Validator for the <see cref="ChangePasswordRequest"/> model.
/// Ensures that passwords meet security requirements and match.
/// </summary>
public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    /// <summary>
    /// Constructor that sets up validation rules for changing passwords, including checks for current password, new password strength, and confirmation match.
    /// </summary>
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(6).WithMessage("New passwords must be at least 6 characters.")
            .Password().WithMessage("New passwords must have at least one digit ('0'-'9'), one lowercase ('a'-'z'), and one uppercase ('A'-'Z').");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirm new password is required")
            .Equal(x => x.NewPassword).WithMessage("New passwords do not match");
    }
}

/// <summary>
/// Validator for the <see cref="ProfileEditRequest"/> model.
/// Handles validation for user profile updates including name, phone, email, and address.
/// </summary>
public class ProfileEditRequestValidator : AbstractValidator<ProfileEditRequest>
{
    /// <summary>
    /// Constructor that sets up validation rules for editing user profiles, ensuring that all fields are properly filled and formatted.
    /// </summary>
    public ProfileEditRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters long");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters long");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MinimumLength(2).WithMessage("Phone number must be at least 2 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(100);
    }
}
