using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;

namespace Master.Application.Features.JobOffers.Commands.CreateJobOffer;

/// <summary>
/// Command for a master to submit a price & schedule proposal for a job post.
/// </summary>
public record CreateJobOfferCommand(Guid MasterId, CreateJobOfferDTO Dto) : IRequest<JobOfferResponseDTO>;

public class CreateJobOfferHandler : IRequestHandler<CreateJobOfferCommand, JobOfferResponseDTO>
{
    private readonly IJobOfferRepository _jobOfferRepository;
    private readonly IJobPostRepository _jobPostRepository;
    private readonly IMapper _mapper;

    public CreateJobOfferHandler(IJobOfferRepository jobOfferRepository, IJobPostRepository jobPostRepository, IMapper mapper)
    {
        _jobOfferRepository = jobOfferRepository;
        _jobPostRepository = jobPostRepository;
        _mapper = mapper;
    }

    public async Task<JobOfferResponseDTO> Handle(CreateJobOfferCommand request, CancellationToken cancellationToken)
    {
        var jobPost = await _jobPostRepository.GetByIdWithDetailsAsync(request.Dto.JobPostId, cancellationToken);
        if (jobPost == null)
        {
            throw new KeyNotFoundException("Job post not found.");
        }

        var offer = new JobOffer
        {
            JobPostId = request.Dto.JobPostId,
            MasterId = request.MasterId,
            CustomerId = jobPost.CustomerId,
            OfferedPrice = request.Dto.OfferedPrice,
            Message = request.Dto.Message,
            ScheduledStartDate = request.Dto.ScheduledStartDate,
            ScheduledEndDate = request.Dto.ScheduledEndDate,
            Status = JobOfferStatus.Pending
        };

        await _jobOfferRepository.AddAsync(offer, cancellationToken);
        await _jobOfferRepository.SaveChangesAsync(cancellationToken);

        var detailedOffer = await _jobOfferRepository.GetWithDetailsAsync(offer.Id, cancellationToken);
        return _mapper.Map<JobOfferResponseDTO>(detailedOffer ?? offer);
    }
}
