using Master.Application.Common;
using Master.Application.Interfaces;
using Master.Domain.Models;
using Master.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Master.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IChatRepository for Chat database operations.
/// </summary>
public class ChatRepository : IChatRepository
{
    private readonly MasterDbContext _context;

    public ChatRepository(MasterDbContext context)
    {
        _context = context;
    }

    public async Task<ChatRoom> CreateOrGetChatRoomAsync(Guid customerId, Guid sellerId, Guid? productId, CancellationToken ct)
    {
        var existingRoom = await _context.ChatRooms
            .Include(r => r.Customer)
            .Include(r => r.Seller)
            .FirstOrDefaultAsync(r => 
                ((r.CustomerId == customerId && r.SellerId == sellerId) ||
                 (r.CustomerId == sellerId && r.SellerId == customerId)) &&
                r.ProductId == productId, ct);

        if (existingRoom != null)
        {
            return existingRoom;
        }

        var newRoom = new ChatRoom
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SellerId = sellerId,
            ProductId = productId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.ChatRooms.Add(newRoom);
        await _context.SaveChangesAsync(ct);

        return (await GetChatRoomByIdAsync(newRoom.Id, ct))!;
    }

    public async Task<ChatRoom?> GetChatRoomByIdAsync(Guid roomId, CancellationToken ct)
    {
        return await _context.ChatRooms
            .Include(r => r.Customer)
            .Include(r => r.Seller)
            .Include(r => r.Messages.OrderByDescending(m => m.SentAt).Take(1))
            .FirstOrDefaultAsync(r => r.Id == roomId, ct);
    }

    public async Task<List<ChatRoom>> GetUserChatRoomsAsync(Guid userId, CancellationToken ct)
    {
        return await _context.ChatRooms
            .Include(r => r.Customer)
            .Include(r => r.Seller)
            .Include(r => r.Messages.OrderByDescending(m => m.SentAt).Take(1))
            .Where(r => r.CustomerId == userId || r.SellerId == userId)
            .OrderByDescending(r => r.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<ChatMessage>> GetRoomMessagesPagedAsync(Guid roomId, int page, int pageSize, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : (pageSize > 100 ? 100 : pageSize);

        var query = _context.ChatMessages
            .Include(m => m.Sender)
            .Where(m => m.ChatRoomId == roomId)
            .OrderByDescending(m => m.SentAt);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        // Reverse items to return chronological order for UI while keeping correct pagination
        items.Reverse();

        return PagedResult<ChatMessage>.Create(items, page, pageSize, totalCount);
    }

    public async Task<ChatMessage> SaveMessageAsync(ChatMessage message, CancellationToken ct)
    {
        _context.ChatMessages.Add(message);

        var room = await _context.ChatRooms.FindAsync(new object[] { message.ChatRoomId }, ct);
        if (room != null)
        {
            room.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync(ct);

        return await _context.ChatMessages
            .Include(m => m.Sender)
            .FirstAsync(m => m.Id == message.Id, ct);
    }

    public async Task<int> MarkMessagesAsReadAsync(Guid roomId, Guid currentUserId, CancellationToken ct)
    {
        var unreadMessages = await _context.ChatMessages
            .Where(m => m.ChatRoomId == roomId && m.SenderId != currentUserId && !m.IsRead)
            .ToListAsync(ct);

        if (!unreadMessages.Any()) return 0;

        foreach (var msg in unreadMessages)
        {
            msg.IsRead = true;
        }

        await _context.SaveChangesAsync(ct);
        return unreadMessages.Count;
    }

    public async Task<int> GetUnreadCountForUserAsync(Guid userId, CancellationToken ct)
    {
        return await _context.ChatMessages
            .Where(m => m.ChatRoom.CustomerId == userId || m.ChatRoom.SellerId == userId)
            .Where(m => m.SenderId != userId && !m.IsRead)
            .CountAsync(ct);
    }
}
