namespace Master.Services;

using System;
using Master.Data;
using Master.DTOs;
using Master.Models;
using Master.Storage; 
using Microsoft.EntityFrameworkCore;

public class UserAttachmentService : IUserAttachmentService
{
    private readonly MasterDbContext _context;
    private readonly IFileStorage _storage;

    public UserAttachmentService(MasterDbContext context, IFileStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<UserAttachmentResponseDto> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        long size,
        Guid userId,
        CancellationToken ct)
    {
        if (!contentType.StartsWith("image/") && contentType != "application/pdf")
        {
            throw new InvalidOperationException("Only image or PDF files are allowed.");
        }

        var folderKey = $"users/{userId}";
        var storedInfo = await _storage.UploadAsync(stream, fileName, contentType, folderKey, ct);

        var attachment = new UserAttachment
        {
            Id = Guid.NewGuid(),
            OriginalFileName = fileName,
            StoredFileName = storedInfo.StoredFileName,
            StorageKey = storedInfo.StorageKey,
            ContentType = contentType,
            Size = storedInfo.Size,
            UploadedUserId = userId,
            UploadedAt = DateTimeOffset.UtcNow
        };

        _context.UserAttachments.Add(attachment);
        await _context.SaveChangesAsync(ct);

        return new UserAttachmentResponseDto
        {
            Id = attachment.Id,
            FileName = attachment.OriginalFileName,
            ContentType = attachment.ContentType,
            Size = attachment.Size,
            UploadedAt = attachment.UploadedAt
        };
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?> DownloadAsync(
        Guid id,
        Guid userId,
        CancellationToken ct)
    {
        var attachment = await _context.UserAttachments
            .FirstOrDefaultAsync(x => x.Id == id && x.UploadedUserId == userId, ct);

        if (attachment is null)
            return null;

        var stream = await _storage.OpenReadAsync(attachment.StorageKey, ct);

        return (stream, attachment.ContentType, attachment.OriginalFileName);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken ct)
    {
        var attachment = await _context.UserAttachments
            .FirstOrDefaultAsync(x => x.Id == id && x.UploadedUserId == userId, ct);

        if (attachment is null)
            return false;

        await _storage.DeleteAsync(attachment.StorageKey, ct);

        _context.UserAttachments.Remove(attachment);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}