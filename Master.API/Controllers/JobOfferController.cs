using System.Security.Claims;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Features.JobOffers.Commands.AcceptJobOffer;
using Master.Application.Features.JobOffers.Commands.CompleteJobOffer;
using Master.Application.Features.JobOffers.Commands.CreateJobOffer;
using Master.Application.Features.JobOffers.Commands.RejectJobOffer;
using Master.Application.Features.JobOffers.Queries.GetJobOffersForJobPost;
using Master.Application.Features.JobOffers.Queries.GetUserJobOffers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Master.API.Controllers;

/// <summary>
/// Controller for managing Job Offers, Proposal Acceptance, and Automated Master Status lifecycle.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class JobOfferController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobOfferController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User is not authenticated."));

    /// <summary>
    /// Submits a new price and date proposal for a job post (Master action).
    /// </summary>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<JobOfferResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<JobOfferResponseDTO>>> CreateJobOffer([FromBody] CreateJobOfferDTO dto)
    {
        var result = await _mediator.Send(new CreateJobOfferCommand(CurrentUserId, dto));
        return Ok(ApiResponse<JobOfferResponseDTO>.SuccessResponse(result, "Job offer submitted successfully."));
    }

    /// <summary>
    /// Retrieves all proposals submitted for a specific job post.
    /// </summary>
    [HttpGet("jobpost/{jobPostId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<JobOfferResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<JobOfferResponseDTO>>>> GetJobPostOffers(Guid jobPostId)
    {
        var result = await _mediator.Send(new GetJobOffersForJobPostQuery(jobPostId));
        return Ok(ApiResponse<List<JobOfferResponseDTO>>.SuccessResponse(result, $"{result.Count} proposal(s) retrieved."));
    }

    /// <summary>
    /// Retrieves proposals associated with the authenticated user (either as Master or Customer).
    /// </summary>
    [Authorize]
    [HttpGet("my-offers")]
    [ProducesResponseType(typeof(ApiResponse<List<JobOfferResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<JobOfferResponseDTO>>>> GetMyJobOffers([FromQuery] bool isMaster = true)
    {
        var result = await _mediator.Send(new GetUserJobOffersQuery(CurrentUserId, isMaster));
        return Ok(ApiResponse<List<JobOfferResponseDTO>>.SuccessResponse(result, $"{result.Count} job offer(s) retrieved."));
    }

    /// <summary>
    /// Accepts a master's job proposal.
    /// Automatically sets JobPost status to InProgress and Master status to Busy!
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/accept")]
    [ProducesResponseType(typeof(ApiResponse<JobOfferResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<JobOfferResponseDTO>>> AcceptOffer(Guid id)
    {
        var result = await _mediator.Send(new AcceptJobOfferCommand(id, CurrentUserId));
        return Ok(ApiResponse<JobOfferResponseDTO>.SuccessResponse(result, "Job offer accepted! Master status is now Busy."));
    }

    /// <summary>
    /// Rejects a master's job proposal.
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(typeof(ApiResponse<JobOfferResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<JobOfferResponseDTO>>> RejectOffer(Guid id)
    {
        var result = await _mediator.Send(new RejectJobOfferCommand(id, CurrentUserId));
        return Ok(ApiResponse<JobOfferResponseDTO>.SuccessResponse(result, "Job offer rejected."));
    }

    /// <summary>
    /// Completes an active job offer.
    /// Automatically sets JobPost status to Completed and reverts Master status back to Available!
    /// </summary>
    [Authorize]
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(typeof(ApiResponse<JobOfferResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<JobOfferResponseDTO>>> CompleteOffer(Guid id)
    {
        var result = await _mediator.Send(new CompleteJobOfferCommand(id, CurrentUserId));
        return Ok(ApiResponse<JobOfferResponseDTO>.SuccessResponse(result, "Job offer completed! Master status is now Available."));
    }
}
