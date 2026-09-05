using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Enums;
using Master.Domain.Models;
using MediatR;

namespace Master.Application.Features.JobOffers.Commands.CompleteJobOffer;

/// <summary>
/// Command to complete an accepted job offer.
/// Automatically sets JobPost -> Completed and Master -> Available!
/// </summary>
public record CompleteJobOfferCommand(Guid OfferId, Guid UserId) : IRequest<JobOfferResponseDTO>;

public class CompleteJobOfferHandler : IRequestHandler<CompleteJobOfferCommand, JobOfferResponseDTO>
{
    private readonly IJobOfferRepository _jobOfferRepository;
    private readonly IJobPostRepository _jobPostRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;

    public CompleteJobOfferHandler(
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

    public async Task<JobOfferResponseDTO> Handle(CompleteJobOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _jobOfferRepository.GetWithDetailsAsync(request.OfferId, cancellationToken);
        if (offer == null)
        {
            throw new KeyNotFoundException("Job offer not found.");
        }

        if (offer.CustomerId != request.UserId && offer.MasterId != request.UserId)
        {
            throw new UnauthorizedAccessException("Only involved master or customer can complete the job offer.");
        }

        if (offer.Status != JobOfferStatus.Accepted)
        {
            throw new InvalidOperationException("Only accepted job offers can be completed.");
        }

        // 1. Mark Offer as Completed
        offer.Status = JobOfferStatus.Completed;
        offer.UpdatedAt = DateTimeOffset.UtcNow;
        _jobOfferRepository.Update(offer);

        // 2. Mark JobPost as Completed
        if (offer.JobPost != null)
        {
            offer.JobPost.JPStatus = JobPostStatus.Completed;
            _jobPostRepository.Update(offer.JobPost);
        }

        // 3. Automatically revert Master status to Available!
        var master = await _authRepository.GetByIdAsync(offer.MasterId, cancellationToken);
        if (master != null)
        {
            master.Status = MasterStatus.Available;
            master.UpdatedAt = DateTimeOffset.UtcNow;
            await _authRepository.UpdateAsync(master);
        }

        await _jobOfferRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<JobOfferResponseDTO>(offer);
    }
}
