using System.Security.Claims;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Features.MasterStatusFeature.Commands.UpdateMasterStatus;
using Master.Application.Features.MasterStatusFeature.Queries.GetAllMasterStatuses;
using Master.Application.Features.MasterStatusFeature.Queries.GetMasterStatus;
using Master.Domain.Constants;
using Master.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Master.API.Controllers;

/// <summary>
/// Controller for managing master availability statuses (Available, Busy, Offline).
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MasterStatusController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of MasterStatusController.
    /// </summary>
    /// <param name="mediator">Mediator instance.</param>
    public MasterStatusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Safely retrieves the authenticated user's ID from claims.
    /// </summary>
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User is not authenticated."));

    /// <summary>
    /// Gets all possible master status options (lookup list for UI dropdowns).
    /// </summary>
    /// <returns>A list of master status lookup items.</returns>
    [HttpGet("statuses")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MasterStatusLookupDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MasterStatusLookupDto>>>> GetAllStatuses()
    {
        var result = await _mediator.Send(new GetAllMasterStatusesQuery());
        return Ok(ApiResponse<IEnumerable<MasterStatusLookupDto>>.SuccessResponse(result, "Master statuses retrieved successfully."));
    }

    /// <summary>
    /// Gets the current availability status of the authenticated master user.
    /// </summary>
    /// <returns>Current master status details.</returns>
    [Authorize]
    [HttpGet("my-status")]
    [ProducesResponseType(typeof(ApiResponse<MasterStatusResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MasterStatusResponseDTO>>> GetMyStatus()
    {
        var result = await _mediator.Send(new GetMasterStatusQuery(UserId));
        return Ok(ApiResponse<MasterStatusResponseDTO>.SuccessResponse(result, "Current master status retrieved successfully."));
    }

    /// <summary>
    /// Gets the availability status of a specific master by user ID.
    /// </summary>
    /// <param name="masterId">The unique ID of the master user.</param>
    /// <returns>Master status details.</returns>
    [HttpGet("{masterId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MasterStatusResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MasterStatusResponseDTO>>> GetMasterStatus(Guid masterId)
    {
        var result = await _mediator.Send(new GetMasterStatusQuery(masterId));
        return Ok(ApiResponse<MasterStatusResponseDTO>.SuccessResponse(result, "Master status retrieved successfully."));
    }

    /// <summary>
    /// Updates the availability status of the authenticated master user.
    /// </summary>
    /// <param name="request">Request containing the new status value (1 = Available, 2 = Busy, 3 = Offline).</param>
    /// <returns>Updated master status details.</returns>
    [Authorize(Policy = AuthPolicies.MasterOrAdmin)]
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<MasterStatusResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MasterStatusResponseDTO>>> UpdateStatus([FromBody] UpdateMasterStatusRequest request)
    {
        var status = !string.IsNullOrWhiteSpace(request.StatusName) 
            ? MasterStatus.FromName(request.StatusName) 
            : MasterStatus.FromId(request.StatusId);

        var result = await _mediator.Send(new UpdateMasterStatusCommand(UserId, status));
        return Ok(ApiResponse<MasterStatusResponseDTO>.SuccessResponse(result, "Master status updated successfully."));
    }
}
