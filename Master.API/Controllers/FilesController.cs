using Master.Application.Common;
using Master.Application.Features.Files.Commands.DeleteFile;
using Master.Application.Features.Files.Commands.UploadFile;
using Master.Application.Features.Files.Queries.GetFiles;
using Master.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Master.API.Controllers
{
    /// <summary>
    /// Controller for managing file uploads, listing, and deletion via S3.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Uploads a file to S3.
        /// </summary>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(ApiResponse<UploadFileResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UploadFileResponse>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<UploadFileResponse>>> UploadFile([FromForm] UploadFileRequest request)
        {
            var command = new UploadFileCommand
            {
                File = request.File,
                FolderName = request.FolderName ?? "uploads"
            };

            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lists all files in the S3 bucket, optionally filtered by folder prefix.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FileMetadata>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<FileMetadata>>>> GetFiles([FromQuery] string? prefix)
        {
            var query = new GetFilesQuery { Prefix = prefix };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Deletes a file from S3 by its URL.
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteFile([FromQuery] string fileUrl)
        {
            var command = new DeleteFileCommand { FileUrl = fileUrl };
            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }

    public class UploadFileRequest
    {
        public IFormFile File { get; set; } = null!;
        public string? FolderName { get; set; }
    }
}
