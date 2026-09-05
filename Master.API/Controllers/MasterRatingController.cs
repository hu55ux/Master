using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Features.MasterRatings.Commands.CreateRating;
using Master.Application.Features.MasterRatings.Commands.DeleteRating;
using Master.Application.Features.MasterRatings.Commands.UpdateRating;
using Master.Application.Features.MasterRatings.Queries.GetRatingsByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Master.API.Controllers;

/// <summary>
/// Handles operations related to master ratings, including creating, updating, deleting, and fetching ratings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MasterRatingController : ControllerBase
{
    private readonly IMediator _mediator;

    public MasterRatingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new rating for a master.
    /// </summary>
    /// <param name="model">The rating details.</param>
    /// <returns>True if successfully created.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> Create([FromBody] CreateMasterRatingDTO model)
    {
        var result = await _mediator.Send(new CreateMasterRatingCommand(model));
        if (result)
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Rating created successfully."));
            
        return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to create rating."));
    }

    /// <summary>
    /// Updates an existing rating.
    /// </summary>
    /// <param name="model">The updated rating details.</param>
    /// <returns>True if successfully updated.</returns>
    [HttpPut]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> Update([FromBody] UpdateMasterRatingDTO model)
    {
        var result = await _mediator.Send(new UpdateMasterRatingCommand(model));
        if (result)
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Rating updated successfully."));
            
        return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to update rating."));
    }

    /// <summary>
    /// Deletes a specific rating given to a master by a customer.
    /// </summary>
    /// <param name="masterId">The ID of the master who was rated.</param>
    /// <param name="customerId">The ID of the customer who gave the rating.</param>
    /// <returns>True if successfully deleted.</returns>
    [HttpDelete("{masterId:guid}/{customerId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid masterId, Guid customerId)
    {
        var result = await _mediator.Send(new DeleteMasterRatingCommand(masterId, customerId));
        if (result)
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Rating deleted successfully."));
            
        return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to delete rating."));
    }

    /// <summary>
    /// Fetches all ratings and comments for a specific master.
    /// </summary>
    /// <param name="masterId">The ID of the master.</param>
    /// <returns>A list of ratings with customer details.</returns>
    [HttpGet("{masterId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<MasterRatingResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<MasterRatingResponseDTO>>>> GetByMasterId(Guid masterId)
    {
        var result = await _mediator.Send(new GetMasterRatingsByUserIdQuery(masterId));
        return Ok(ApiResponse<List<MasterRatingResponseDTO>>.SuccessResponse(result ?? new List<MasterRatingResponseDTO>()));
    }

    /// <summary>
    /// Fetches all ratings and comments for a specific user (master).
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of ratings with customer details.</returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<MasterRatingResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<MasterRatingResponseDTO>>>> GetByUserId(Guid userId)
    {
        var result = await _mediator.Send(new GetMasterRatingsByUserIdQuery(userId));
        return Ok(ApiResponse<List<MasterRatingResponseDTO>>.SuccessResponse(result ?? new List<MasterRatingResponseDTO>()));
    }
}
