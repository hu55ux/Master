using Master.DTOs;
using Master.Models;

namespace Master.Services;

public interface IUserAttachmentService
{
    Task<UserAttachmentResponseDto> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        long size,
        Guid userId,
        CancellationToken ct);

    Task<(Stream Stream, string ContentType, string FileName)?> DownloadAsync(
        Guid id,
        Guid userId,
        CancellationToken ct);

    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken ct);
}