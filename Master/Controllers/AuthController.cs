using System.Security.Claims;
using Master.Common;
using Master.DTOs;
using Master.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Master.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> with the required authentication service.
    /// </summary>
    /// <param name="authService">The service handling authentication logic.</param>
    public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor)
    {
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Registers a new user account within the system.
    /// </summary>
    /// <param name="request">The user's registration details (email, password, etc.).</param>
    /// <returns>A standard API response containing the authentication result and tokens.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterUserAsync(request);
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// </summary>
    /// <param name="loginRequest">User login credentials (email and password)</param>
    /// <returns>
    /// Authentication response containing access and refresh tokens.
    /// </returns>
    /// <response code="200">User successfully authenticated</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await _authService.LoginUserAsync(loginRequest);

        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenRequest);

        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    }

    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        await _authService.RevokeRefreshTokenAsync(refreshTokenRequest.RefreshToken);

        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse("Refresh token revoked"));
    }

    /// <summary>
    /// Edits the profile information of the currently authenticated user. 
    /// This endpoint allows users to update their personal details such as name, email, address, and phone number.
    /// </summary>
    /// <param name="request">The profile update data.</param>
    /// <returns>A success response containing the updated user details and new tokens.</returns>
    [HttpPut("edit-profile")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> EditOwnProfile([FromBody] ProfileEditRequest request)
    {
        var userId = _authService.UserId;

        var result = await _authService.EditOwnProfileAsync(userId, request);

        if (result == null)
        {
            return BadRequest(ApiResponse<AuthResponseDTO>.ErrorResponse("Could not update profile."));
        }

        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Profile updated successfully."));
    }


    /// <summary>
    /// Changes the password of the currently authenticated user. This endpoint requires the user to provide their current password for verification
    /// , along with the new password and its confirmation. Upon successful validation, the user's password will be updated in the system.
    /// </summary>
    /// <param name="request">The object containing current and new password details.</param>
    /// <returns>An <see cref="ApiResponse{AuthResponseDTO}"/> with the operation result.</returns>
    [HttpPut("change-password")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> ChangeOwnPassword([FromBody] ChangePasswordRequest request)
    {
        var userId = _authService.UserId;

        var result = await _authService.ChangePasswordAsync(userId, request);

        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    }

    /// <summary>
    /// Deletes the authenticated user's own profile and all associated data permanently.
    /// </summary>
    /// <remarks>
    /// This endpoint triggers a hard delete. Once executed, the user data cannot be recovered.
    /// Access is restricted to authenticated users only.
    /// </remarks>
    /// <returns>A success message wrapped in ApiResponse.</returns>
    [HttpDelete("delete-own-profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> DeleteOwnProfile()
    {
        var userId = _authService.UserId;

        await _authService.DeleteOwnProfile(userId);

        return Ok(ApiResponse<string>.SuccessResponse("Profile permanently deleted.", "Success"));
    }


}
