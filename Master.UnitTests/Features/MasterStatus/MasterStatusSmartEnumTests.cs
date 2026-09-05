using FluentAssertions;
using Master.Application.DTOs;
using Master.Application.Features.MasterStatusFeature.Commands.UpdateMasterStatus;
using Master.Application.Features.MasterStatusFeature.Queries.GetMasterStatus;
using Master.Application.Interfaces;
using Master.Domain.Enums;
using Master.Domain.Models;
using Moq;
using Xunit;

namespace Master.UnitTests.Features.MasterStatusFeature;

public class MasterStatusSmartEnumTests
{
    [Fact]
    public void MasterStatus_Should_HaveCorrectValuesAndProperties()
    {
        MasterStatus.Available.Id.Should().Be(1);
        MasterStatus.Available.Name.Should().Be("Available");
        MasterStatus.Available.DisplayName.Should().Be("Available");
        MasterStatus.Available.ColorCode.Should().Be("#22C55E");
        MasterStatus.Available.CanAcceptJobs.Should().BeTrue();

        MasterStatus.Busy.Id.Should().Be(2);
        MasterStatus.Busy.CanAcceptJobs.Should().BeFalse();

        MasterStatus.Offline.Id.Should().Be(3);
        MasterStatus.Offline.CanAcceptJobs.Should().BeFalse();
    }

    [Fact]
    public void MasterStatus_FromId_And_FromName_Should_ResolveCorrectInstances()
    {
        var fromId = MasterStatus.FromId(1);
        var fromName = MasterStatus.FromName("Available");

        fromId.Should().Be(MasterStatus.Available);
        fromName.Should().Be(MasterStatus.Available);
        (fromId == fromName).Should().BeTrue();
    }

    [Fact]
    public void MasterStatus_FromId_Should_ThrowException_When_InvalidId()
    {
        Action act = () => MasterStatus.FromId(99);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}

public class UpdateMasterStatusHandlerTests
{
    private readonly Mock<IAuthRepository> _authRepoMock;
    private readonly UpdateMasterStatusHandler _handler;

    public UpdateMasterStatusHandlerTests()
    {
        _authRepoMock = new Mock<IAuthRepository>();
        _handler = new UpdateMasterStatusHandler(_authRepoMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateMasterStatus_Successfully()
    {
        var masterId = Guid.NewGuid();
        var masterUser = new AppUser { Id = masterId, FirstName = "Ali", LastName = "Valiyev", Status = MasterStatus.Offline };
        var command = new UpdateMasterStatusCommand(masterId, MasterStatus.Available);

        _authRepoMock.Setup(x => x.GetByIdAsync(masterId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(masterUser);

        _authRepoMock.Setup(x => x.UpdateAsync(masterUser))
                     .ReturnsAsync(Microsoft.AspNetCore.Identity.IdentityResult.Success);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.MasterId.Should().Be(masterId);
        result.StatusId.Should().Be(1);
        result.StatusName.Should().Be("Available");
        result.CanAcceptJobs.Should().BeTrue();
        masterUser.Status.Should().Be(MasterStatus.Available);
        _authRepoMock.Verify(x => x.UpdateAsync(masterUser), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowKeyNotFoundException_When_UserDoesNotExist()
    {
        var masterId = Guid.NewGuid();
        var command = new UpdateMasterStatusCommand(masterId, MasterStatus.Busy);

        _authRepoMock.Setup(x => x.GetByIdAsync(masterId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((AppUser)null!);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage("Master user not found.");
    }
}
