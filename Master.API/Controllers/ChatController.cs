using System.Security.Claims;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Features.Chat.Commands.CreateOrGetChatRoom;
using Master.Application.Features.Chat.Commands.MarkMessagesAsRead;
using Master.Application.Features.Chat.Queries.GetRoomMessages;
using Master.Application.Features.Chat.Queries.GetUserChatRooms;
using Master.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Master.API.Controllers;

/// <summary>
/// Controller for Chat Module endpoints (Dialogue management, message pagination, read status).
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IRedisChatService _redisChatService;
    private readonly IFileService _fileService;

    public ChatController(IMediator mediator, IRedisChatService redisChatService, IFileService fileService)
    {
        _mediator = mediator;
        _redisChatService = redisChatService;
        _fileService = fileService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User is not authenticated."));

    /// <summary>
    /// Creates or retrieves an existing chat room with a seller for a product context.
    /// </summary>
    [HttpPost("rooms")]
    [ProducesResponseType(typeof(ApiResponse<ChatRoomResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ChatRoomResponseDTO>>> CreateOrGetRoom([FromBody] CreateOrGetChatRoomRequest request)
    {
        var result = await _mediator.Send(new CreateOrGetChatRoomCommand(CurrentUserId, request.SellerId, request.ProductId));
        return Ok(ApiResponse<ChatRoomResponseDTO>.SuccessResponse(result, "Chat room retrieved or created successfully."));
    }

    /// <summary>
    /// Retrieves all chat rooms/dialogues for the authenticated user.
    /// </summary>
    [HttpGet("rooms")]
    [ProducesResponseType(typeof(ApiResponse<List<ChatRoomResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ChatRoomResponseDTO>>>> GetUserChatRooms()
    {
        var result = await _mediator.Send(new GetUserChatRoomsQuery(CurrentUserId));
        return Ok(ApiResponse<List<ChatRoomResponseDTO>>.SuccessResponse(result, "Chat rooms retrieved successfully."));
    }

    /// <summary>
    /// Retrieves paginated chat message history for a specific room.
    /// </summary>
    [HttpGet("rooms/{roomId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ChatMessageResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ChatMessageResponseDTO>>>> GetRoomMessages(
        Guid roomId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetRoomMessagesQuery(roomId, page, pageSize));
        return Ok(ApiResponse<PagedResult<ChatMessageResponseDTO>>.SuccessResponse(result, "Messages retrieved successfully."));
    }

    /// <summary>
    /// Marks all unread messages in a room as read by the authenticated user.
    /// </summary>
    [HttpPut("rooms/{roomId:guid}/read")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<int>>> MarkMessagesAsRead(Guid roomId)
    {
        var readCount = await _mediator.Send(new MarkMessagesAsReadCommand(roomId, CurrentUserId));
        return Ok(ApiResponse<int>.SuccessResponse(readCount, $"{readCount} message(s) marked as read."));
    }

    /// <summary>
    /// Gets the real-time online status of a target user from Redis.
    /// </summary>
    [HttpGet("users/{userId:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> GetUserOnlineStatus(Guid userId)
    {
        var isOnline = await _redisChatService.IsUserOnlineAsync(userId);
        return Ok(ApiResponse<bool>.SuccessResponse(isOnline, "User status retrieved successfully."));
    }

    /// <summary>
    /// Uploads an image or file to Cloudinary CDN for chat sharing.
    /// </summary>
    /// <param name="file">The image or media file from mobile/web form data.</param>
    /// <returns>Cloudinary CDN secure file URL and metadata.</returns>
    [HttpPost("upload-media")]
    [ProducesResponseType(typeof(ApiResponse<UploadMediaResponseDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UploadMediaResponseDTO>>> UploadChatMedia(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<UploadMediaResponseDTO>.ErrorResponse("No file was uploaded."));
        }

        var secureUrl = await _fileService.UploadFileAsync(file, "chat-media");
        var isImage = file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

        var dto = new UploadMediaResponseDTO
        {
            FileUrl = secureUrl,
            FileName = file.FileName,
            FileSize = file.Length,
            MediaType = isImage ? "image" : "document"
        };

        return Ok(ApiResponse<UploadMediaResponseDTO>.SuccessResponse(dto, "Media uploaded to Cloudinary successfully."));
    }
}
