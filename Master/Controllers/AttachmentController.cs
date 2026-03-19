using System.Security.Claims;
using Master.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserAttachmentsController : ControllerBase
{
    private readonly IUserAttachmentService _service;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public UserAttachmentsController(IUserAttachmentService service)
    {
        _service = service;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        await using var stream = file.OpenReadStream();

        var result = await _service.UploadAsync(
            stream,
            file.FileName,
            file.ContentType,
            file.Length,
            UserId,
            ct
        );

        return Ok(result);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var file = await _service.DownloadAsync(id, UserId, ct);

        if (file is null)
            return NotFound();

        return File(file.Value.Stream, file.Value.ContentType, file.Value.FileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, UserId, ct);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}