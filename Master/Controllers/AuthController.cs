using System.Security.Claims;
using Master.Common;
using Master.DTOs;
using Master.Features.Authorization.Commands;
using Master.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Master.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Safely retrieves the authenticated user's ID from claims.
    /// </summary>
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User is not authenticated."));

    /// <summary>
    /// Registers a new user account within the system. This endpoint accepts user registration details such as email, password, and other relevant information
    /// Upon successful registration, it returns an authentication response containing access and refresh tokens for the newly created account.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register([FromBody] RegisterRequest request)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Registration completed successfully."));
    }

    /// <summary>
    /// Logins a user by validating their credentials and returns JWT tokens. This endpoint accepts the 
    /// user's email and password, verifies them against the stored credentials,
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Login successful."));
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token. This endpoint accepts a refresh token, validates it, 
    /// and if valid, issues a new access token along with a new refresh token.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    }

    /// <summary>
    /// Revokes a refresh token, effectively logging the user out from all sessions that rely on that token.
    /// This endpoint accepts a refresh token and invalidates it in the system,
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke([FromBody] RefreshTokenRequest request)
    {
        await _mediator.Send(new RevokeTokenCommand(request.RefreshToken));
        return Ok(ApiResponse<string>.SuccessResponse("Refresh token revoked successfully."));
    }

    /// <summary>
    /// Edits the profile information of the currently authenticated user. This endpoint 
    /// allows users to update their personal details such as name, email, address, and phone number.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("editProfile")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> EditOwnProfile([FromBody] ProfileEditRequest request)
    {
        var result = await _mediator.Send(new EditProfileCommand(UserId, request));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Profile updated successfully."));
    }

    /// <summary>
    /// changes the password of the currently authenticated user. This endpoint requires the user to provide their current password for verification,
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("changePassword")]
    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> ChangeOwnPassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _mediator.Send(new ChangePasswordCommand(UserId, request));
        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Password changed successfully."));
    }

    /// <summary>
    /// Deletes the authenticated user's own profile and all associated data permanently. 
    /// This endpoint triggers a hard delete, meaning that once executed, the user data cannot be recovered.
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpDelete("deleteProfile")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteOwnProfile()
    {
        await _mediator.Send(new DeleteProfileCommand(UserId));
        return Ok(ApiResponse<string>.SuccessResponse("Profile and all associated data deleted permanently."));
    }

    
    //    /// <summary>
    //    /// Registers a new user account within the system.
    //    /// </summary>
    //    /// <param name="request">The user's registration details (email, password, etc.).</param>
    //    /// <returns>A standard API response containing the authentication result and tokens.</returns>
    //    [HttpPost("register")]
    //    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    //    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    //    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register([FromBody] RegisterRequest request)
    //    {
    //        var result = await _authService.RegisterUserAsync(request);
    //        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    //    }

    //    /// <summary>
    //    /// Authenticates a user and returns JWT tokens.
    //    /// </summary>
    //    /// <param name="loginRequest">User login credentials (email and password)</param>
    //    /// <returns>
    //    /// Authentication response containing access and refresh tokens.
    //    /// </returns>
    //    /// <response code="200">User successfully authenticated</response>
    //    /// <response code="401">Invalid credentials</response>
    //    [HttpPost("login")]
    //    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginRequest loginRequest)
    //    {
    //        var result = await _authService.LoginUserAsync(loginRequest);

    //        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    //    }

    //    [HttpPost("refresh")]
    //    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
    //    {
    //        var result = await _authService.RefreshTokenAsync(refreshTokenRequest);

    //        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    //    }

    //    [HttpPost("revoke")]
    //    public async Task<ActionResult> Revoke([FromBody] RefreshTokenRequest refreshTokenRequest)
    //    {
    //        await _authService.RevokeRefreshTokenAsync(refreshTokenRequest.RefreshToken);

    //        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse("Refresh token revoked"));
    //    }

    //    /// <summary>
    //    /// Edits the profile information of the currently authenticated user. 
    //    /// This endpoint allows users to update their personal details such as name, email, address, and phone number.
    //    /// </summary>
    //    /// <param name="request">The profile update data.</param>
    //    /// <returns>A success response containing the updated user details and new tokens.</returns>
    //    [HttpPut("edit-profile")]
    //    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    //    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    //    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> EditOwnProfile([FromBody] ProfileEditRequest request)
    //    {
    //        var userId = _authService.UserId;

    //        var result = await _authService.EditOwnProfileAsync(userId, request);

    //        if (result == null)
    //        {
    //            return BadRequest(ApiResponse<AuthResponseDTO>.ErrorResponse("Could not update profile."));
    //        }

    //        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Profile updated successfully."));
    //    }


    //    /// <summary>
    //    /// Changes the password of the currently authenticated user. This endpoint requires the user to provide their current password for verification
    //    /// , along with the new password and its confirmation. Upon successful validation, the user's password will be updated in the system.
    //    /// </summary>
    //    /// <param name="request">The object containing current and new password details.</param>
    //    /// <returns>An <see cref="ApiResponse{AuthResponseDTO}"/> with the operation result.</returns>
    //    [HttpPut("change-password")]
    //    [ProducesResponseType(typeof(ApiResponse<AuthResponseDTO>), StatusCodes.Status200OK)]
    //    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    //    public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> ChangeOwnPassword([FromBody] ChangePasswordRequest request)
    //    {
    //        var userId = _authService.UserId;

    //        var result = await _authService.ChangePasswordAsync(userId, request);

    //        return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result));
    //    }

    //    /// <summary>
    //    /// Deletes the authenticated user's own profile and all associated data permanently.
    //    /// </summary>
    //    /// <remarks>
    //    /// This endpoint triggers a hard delete. Once executed, the user data cannot be recovered.
    //    /// Access is restricted to authenticated users only.
    //    /// </remarks>
    //    /// <returns>A success message wrapped in ApiResponse.</returns>
    //    [HttpDelete("delete-own-profile")]
    //    [Authorize]
    //    public async Task<ActionResult<ApiResponse<string>>> DeleteOwnProfile()
    //    {
    //        var userId = _authService.UserId;

    //        await _authService.DeleteOwnProfile(userId);

    //        return Ok(ApiResponse<string>.SuccessResponse("Profile permanently deleted.", "Success"));
    //    }


}
