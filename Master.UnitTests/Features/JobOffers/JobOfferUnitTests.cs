using AutoMapper;
using FluentAssertions;
using Master.Application.DTOs;
using Master.Application.Features.JobOffers.Commands.AcceptJobOffer;
using Master.Application.Features.JobOffers.Commands.CompleteJobOffer;
using Master.Application.Interfaces;
using Master.Domain.Enums;
using Master.Domain.Models;
using Moq;
using Xunit;

namespace Master.UnitTests.Features.JobOffers;

public class JobOfferUnitTests
{
    private readonly Mock<IJobOfferRepository> _jobOfferRepositoryMock = new();
    private readonly Mock<IJobPostRepository> _jobPostRepositoryMock = new();
    private readonly Mock<IAuthRepository> _authRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task AcceptJobOffer_Should_Set_JobPost_InProgress_And_Master_Status_Busy()
    {
        var offerId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var masterId = Guid.NewGuid();

        var jobPost = new JobPost { Id = Guid.NewGuid(), CustomerId = customerId, JPStatus = JobPostStatus.Pending };
        var masterUser = new AppUser { Id = masterId, Status = MasterStatus.Available };
        var offer = new JobOffer
        {
            Id = offerId,
            CustomerId = customerId,
            MasterId = masterId,
            Status = JobOfferStatus.Pending,
            JobPost = jobPost
        };

        _jobOfferRepositoryMock.Setup(r => r.GetWithDetailsAsync(offerId, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
        _authRepositoryMock.Setup(r => r.GetByIdAsync(masterId, It.IsAny<CancellationToken>())).ReturnsAsync(masterUser);

        var handler = new AcceptJobOfferHandler(_jobOfferRepositoryMock.Object, _jobPostRepositoryMock.Object, _authRepositoryMock.Object, _mapperMock.Object);
        await handler.Handle(new AcceptJobOfferCommand(offerId, customerId), CancellationToken.None);

        offer.Status.Should().Be(JobOfferStatus.Accepted);
        jobPost.JPStatus.Should().Be(JobPostStatus.InProgress);
        masterUser.Status.Should().Be(MasterStatus.Busy);
    }

    [Fact]
    public async Task CompleteJobOffer_Should_Set_JobPost_Completed_And_Master_Status_Available()
    {
        var offerId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var masterId = Guid.NewGuid();

        var jobPost = new JobPost { Id = Guid.NewGuid(), CustomerId = customerId, JPStatus = JobPostStatus.InProgress };
        var masterUser = new AppUser { Id = masterId, Status = MasterStatus.Busy };
        var offer = new JobOffer
        {
            Id = offerId,
            CustomerId = customerId,
            MasterId = masterId,
            Status = JobOfferStatus.Accepted,
            JobPost = jobPost
        };

        _jobOfferRepositoryMock.Setup(r => r.GetWithDetailsAsync(offerId, It.IsAny<CancellationToken>())).ReturnsAsync(offer);
        _authRepositoryMock.Setup(r => r.GetByIdAsync(masterId, It.IsAny<CancellationToken>())).ReturnsAsync(masterUser);

        var handler = new CompleteJobOfferHandler(_jobOfferRepositoryMock.Object, _jobPostRepositoryMock.Object, _authRepositoryMock.Object, _mapperMock.Object);
        await handler.Handle(new CompleteJobOfferCommand(offerId, customerId), CancellationToken.None);

        offer.Status.Should().Be(JobOfferStatus.Completed);
        jobPost.JPStatus.Should().Be(JobPostStatus.Completed);
        masterUser.Status.Should().Be(MasterStatus.Available);
    }
}
