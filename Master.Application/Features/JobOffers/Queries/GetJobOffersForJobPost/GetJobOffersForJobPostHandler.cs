using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.JobOffers.Queries.GetJobOffersForJobPost;

public record GetJobOffersForJobPostQuery(Guid JobPostId) : IRequest<List<JobOfferResponseDTO>>;

public class GetJobOffersForJobPostHandler : IRequestHandler<GetJobOffersForJobPostQuery, List<JobOfferResponseDTO>>
{
    private readonly IJobOfferRepository _jobOfferRepository;
    private readonly IMapper _mapper;

    public GetJobOffersForJobPostHandler(IJobOfferRepository jobOfferRepository, IMapper mapper)
    {
        _jobOfferRepository = jobOfferRepository;
        _mapper = mapper;
    }

    public async Task<List<JobOfferResponseDTO>> Handle(GetJobOffersForJobPostQuery request, CancellationToken cancellationToken)
    {
        var offers = await _jobOfferRepository.GetByJobPostIdAsync(request.JobPostId, cancellationToken);
        return _mapper.Map<List<JobOfferResponseDTO>>(offers);
    }
}
