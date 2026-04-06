using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.MasterRatings.Queries.GetRatings;

public class GetMasterRatingsHandler : IRequestHandler<GetMasterRatingsQuery, List<MasterRatingResponseDTO>>
{
    private readonly IMasterRatingRepository _ratingRepository;
    private readonly IMapper _mapper;

    public GetMasterRatingsHandler(IMasterRatingRepository ratingRepository, IMapper mapper)
    {
        _ratingRepository = ratingRepository;
        _mapper = mapper;
    }

    public async Task<List<MasterRatingResponseDTO>> Handle(GetMasterRatingsQuery request, CancellationToken ct)
    {
        var ratings = await _ratingRepository.GetByMasterIdAsync(request.MasterId, ct);
        
        return _mapper.Map<List<MasterRatingResponseDTO>>(ratings);
    }
}
