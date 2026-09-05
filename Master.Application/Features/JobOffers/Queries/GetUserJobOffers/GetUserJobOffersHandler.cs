using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.JobOffers.Queries.GetUserJobOffers;

/// <summary>
/// Query to retrieve all proposals submitted or received by the current user.
/// </summary>
public record GetUserJobOffersQuery(Guid UserId, bool IsMaster) : IRequest<List<JobOfferResponseDTO>>;

public class GetUserJobOffersHandler : IRequestHandler<GetUserJobOffersQuery, List<JobOfferResponseDTO>>
{
    private readonly IJobOfferRepository _jobOfferRepository;
    private readonly IMapper _mapper;

    public GetUserJobOffersHandler(IJobOfferRepository jobOfferRepository, IMapper mapper)
    {
        _jobOfferRepository = jobOfferRepository;
        _mapper = mapper;
    }

    public async Task<List<JobOfferResponseDTO>> Handle(GetUserJobOffersQuery request, CancellationToken cancellationToken)
    {
        var offers = request.IsMaster
            ? await _jobOfferRepository.GetByMasterIdAsync(request.UserId, cancellationToken)
            : await _jobOfferRepository.GetByCustomerIdAsync(request.UserId, cancellationToken);

        return _mapper.Map<List<JobOfferResponseDTO>>(offers);
    }
}
