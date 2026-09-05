using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Enums;
using Master.Domain.Models;
using MediatR;

namespace Master.Application.Features.JobOffers.Commands.AcceptJobOffer;

/// <summary>
/// Command for a customer to accept a master's job proposal.
/// Automatically sets JobPost -> InProgress and Master -> Busy!
/// </summary>
public record AcceptJobOfferCommand(Guid OfferId, Guid CustomerId) : IRequest<JobOfferResponseDTO>;

public class AcceptJobOfferHandler : IRequestHandler<AcceptJobOfferCommand, JobOfferResponseDTO>
{
    private readonly IJobOfferRepository _jobOfferRepository;
    private readonly IJobPostRepository _jobPostRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;

    public AcceptJobOfferHandler(
        IJobOfferRepository jobOfferRepository,
        IJobPostRepository jobPostRepository,
        IAuthRepository authRepository,
        IMapper mapper)
    {
        _jobOfferRepository = jobOfferRepository;
        _jobPostRepository = jobPostRepository;
        _authRepository = authRepository;
        _mapper = mapper;
    }

    public async Task<JobOfferResponseDTO> Handle(AcceptJobOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _jobOfferRepository.GetWithDetailsAsync(request.OfferId, cancellationToken);
        if (offer == null)
        {
            throw new KeyNotFoundException("Job offer not found.");
        }

        if (offer.CustomerId != request.CustomerId)
        {
            throw new UnauthorizedAccessException("Only the customer who posted the job can accept proposals.");
        }

        if (offer.Status != JobOfferStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot accept offer in {offer.Status} status.");
        }

        // 1. Accept the offer
        offer.Status = JobOfferStatus.Accepted;
        offer.UpdatedAt = DateTimeOffset.UtcNow;
        _jobOfferRepository.Update(offer);

        // 2. Update JobPost status to InProgress
        if (offer.JobPost != null)
        {
            offer.JobPost.JPStatus = JobPostStatus.InProgress;
            _jobPostRepository.Update(offer.JobPost);
        }

        // 3. Automatically update Master status to Busy!
        var master = await _authRepository.GetByIdAsync(offer.MasterId, cancellationToken);
        if (master != null)
        {
            master.Status = MasterStatus.Busy;
            master.UpdatedAt = DateTimeOffset.UtcNow;
            await _authRepository.UpdateAsync(master);
        }

        await _jobOfferRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<JobOfferResponseDTO>(offer);
    }
}
