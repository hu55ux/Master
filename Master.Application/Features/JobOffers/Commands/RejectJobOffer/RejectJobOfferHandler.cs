using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;

namespace Master.Application.Features.JobOffers.Commands.RejectJobOffer;

public record RejectJobOfferCommand(Guid OfferId, Guid CustomerId) : IRequest<JobOfferResponseDTO>;

public class RejectJobOfferHandler : IRequestHandler<RejectJobOfferCommand, JobOfferResponseDTO>
{
    private readonly IJobOfferRepository _jobOfferRepository;
    private readonly IMapper _mapper;

    public RejectJobOfferHandler(IJobOfferRepository jobOfferRepository, IMapper mapper)
    {
        _jobOfferRepository = jobOfferRepository;
        _mapper = mapper;
    }

    public async Task<JobOfferResponseDTO> Handle(RejectJobOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _jobOfferRepository.GetWithDetailsAsync(request.OfferId, cancellationToken);
        if (offer == null)
        {
            throw new KeyNotFoundException("Job offer not found.");
        }

        if (offer.CustomerId != request.CustomerId)
        {
            throw new UnauthorizedAccessException("Only the job post owner can reject offers.");
        }

        offer.Status = JobOfferStatus.Rejected;
        offer.UpdatedAt = DateTimeOffset.UtcNow;

        _jobOfferRepository.Update(offer);
        await _jobOfferRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<JobOfferResponseDTO>(offer);
    }
}
