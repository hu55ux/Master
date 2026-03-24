namespace Master.Application.DTOs;

/// <summary>
/// RegisterRequest: This class represents the data transfer object (DTO) for user registration.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// FirstName
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// LastName
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    /// <example>Pass123</example>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmed Password
    /// </summary>
    /// <example>Pass123</example>
    public string ConfirmedPassword { get; set; } = string.Empty;

    /// <summary>
    /// Date of Birth of the user. This property is used to store the user's date of birth during 
    /// registration, which can be used for age verification, personalization, or other purposes within the application.
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Experience of the user. This property is used to store the user's experience level 
    /// during registration, which can be used for personalization, job matching, or other purposes within the application.
    /// </summary>
    public short Experience { get; set; }

    /// <summary>
    /// This property is used to store the contact phone number of the user during registration.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// This property is used to store the physical address of the user during registration.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// This property is used to store users role during registration.
    /// </summary>
    public string Role { get; set; } = string.Empty;
}


/// <summary>
/// LoginRequest: This class represents the data transfer object (DTO) for user login.
/// </summary>
public class LoginRequest
{

    /// <summary>
    /// Gets or sets the email address associated with the user.
    /// </summary>
    /// <example>someString@gmail.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password of the user. This property 
    /// is required for login and is used to authenticate the user when they log in to the system.
    /// </summary>
    /// <example> "Password123!"</example>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// AuthResponseDTO: This class represents the data transfer object 
/// (DTO) for the response returned after a successful authentication (login).
/// </summary>
public class AuthResponseDTO
{
    /// <summary>
    /// User identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token Expires date 
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// RefreshToken Expires date 
    /// </summary>
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    /// <summary>
    /// User's email address. This property is used to store the email address of the authenticated user, which can be displayed in the user interface or used for personalization purposes after successful authentication.
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name of the user. This property is used to store the user's first name, which can be displayed in the user interface or used for personalization purposes after successful authentication.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the user. This property is used to store the user's last name, which can be displayed in the user interface or used for personalization purposes after successful authentication.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Address of the user. This property is used to store the user's physical address, which can be displayed in the user interface or used for personalization purposes after successful authentication.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the user. This property is used to store the user's contact phone number, which can be displayed in the user interface or used for personalization purposes after successful authentication.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
    /// <summary>
    /// Experience of the user. This property is used to store the user's experience level, which can be displayed in the user interface or used for personalization purposes after successful authentication.
    /// </summary>
    public short Experience { get; set; }
    /// <summary>
    /// Date of birth of the user. This property is used to store the user's date of birth, which can be displayed in the user interface or used for personalization purposes after successful authentication.
    /// </summary>
    public DateTimeOffset DateOfBirth { get; set; }

    /// <summary>
    /// Roles of the user. This property is a collection of 
    /// strings that represents the roles assigned to the authenticated user.
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

/// <summary>
/// RefreshTokenRequest: This class represents the data transfer object 
/// (DTO) for requesting a new access token using a refresh token.
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// String property named "RefreshToken" that is used to store the refresh token value.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Represents a request to update user profile information.
/// </summary>
public class ProfileEditRequest
{
    /// <summary>
    /// The new full name of the user.
    /// </summary>
    /// <example>John Doe</example>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the user. This property is used to store the user's last name, which can be displayed in the user interface or used for personalization purposes after successful authentication.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The new physical address of the user.
    /// </summary>
    /// <example>123 Main St, New York, NY</example>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// The new primary email address for the user account.
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The new contact phone number of the user.
    /// </summary>
    /// <example>+1234567890</example>
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// Represents a request to change the current user's password.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// The user's current password for security verification.
    /// </summary>
    /// <example>OldP@ssw0rd123</example>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// The new password that the user wants to set.
    /// </summary>
    /// <example>NewSecur3P@ss!</example>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// A confirmation of the new password to ensure it was typed correctly.
    /// </summary>
    /// <example>NewSecur3P@ss!</example>
    public string ConfirmNewPassword { get; set; } = string.Empty;

}
