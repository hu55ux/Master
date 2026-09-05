using FluentAssertions;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Features.Chat.Commands.CreateOrGetChatRoom;
using Master.Application.Features.Chat.Commands.MarkMessagesAsRead;
using Master.Application.Features.Chat.Queries.GetRoomMessages;
using Master.Application.Features.Chat.Queries.GetUserChatRooms;
using Master.Application.Interfaces;
using Master.Domain.Enums;
using Master.Domain.Models;
using Moq;
using Xunit;

namespace Master.UnitTests.Features.Chat;

public class ChatFeatureTests
{
    private readonly Mock<IChatRepository> _chatRepoMock;
    private readonly Mock<IRedisChatService> _redisServiceMock;

    public ChatFeatureTests()
    {
        _chatRepoMock = new Mock<IChatRepository>();
        _redisServiceMock = new Mock<IRedisChatService>();
    }

    [Fact]
    public async Task CreateOrGetChatRoomHandler_Should_ReturnChatRoomDTO_When_RoomIsCreatedOrFound()
    {
        var customerId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        var room = new ChatRoom
        {
            Id = roomId,
            CustomerId = customerId,
            SellerId = sellerId,
            ProductId = productId,
            Customer = new AppUser { FirstName = "Customer", LastName = "User" },
            Seller = new AppUser { FirstName = "Seller", LastName = "User" }
        };

        _chatRepoMock.Setup(x => x.CreateOrGetChatRoomAsync(customerId, sellerId, productId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(room);

        _redisServiceMock.Setup(x => x.IsUserOnlineAsync(sellerId))
                         .ReturnsAsync(true);

        var handler = new CreateOrGetChatRoomHandler(_chatRepoMock.Object, _redisServiceMock.Object);
        var result = await handler.Handle(new CreateOrGetChatRoomCommand(customerId, sellerId, productId), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(roomId);
        result.CustomerId.Should().Be(customerId);
        result.SellerId.Should().Be(sellerId);
        result.IsPartnerOnline.Should().BeTrue();
        _chatRepoMock.Verify(x => x.CreateOrGetChatRoomAsync(customerId, sellerId, productId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRoomMessagesHandler_Should_ReturnPagedMessages()
    {
        var roomId = Guid.NewGuid();
        var senderId = Guid.NewGuid();

        var messages = new List<ChatMessage>
        {
            new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatRoomId = roomId,
                SenderId = senderId,
                MessageText = "Hello Seller!",
                Type = MessageType.Text,
                SentAt = DateTimeOffset.UtcNow,
                IsRead = true,
                Sender = new AppUser { FirstName = "Ali", LastName = "Valiyev" }
            }
        };

        var pagedMessages = PagedResult<ChatMessage>.Create(messages, 1, 20, 1);

        _chatRepoMock.Setup(x => x.GetRoomMessagesPagedAsync(roomId, 1, 20, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(pagedMessages);

        var handler = new GetRoomMessagesHandler(_chatRepoMock.Object);
        var result = await handler.Handle(new GetRoomMessagesQuery(roomId, 1, 20), CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().MessageText.Should().Be("Hello Seller!");
        result.Items.First().SenderName.Should().Be("Ali Valiyev");
        _chatRepoMock.Verify(x => x.GetRoomMessagesPagedAsync(roomId, 1, 20, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkMessagesAsReadHandler_Should_ReturnUpdatedReadCount()
    {
        var roomId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _chatRepoMock.Setup(x => x.MarkMessagesAsReadAsync(roomId, userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(3);

        var handler = new MarkMessagesAsReadHandler(_chatRepoMock.Object);
        var result = await handler.Handle(new MarkMessagesAsReadCommand(roomId, userId), CancellationToken.None);

        result.Should().Be(3);
        _chatRepoMock.Verify(x => x.MarkMessagesAsReadAsync(roomId, userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
