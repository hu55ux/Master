using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.MasterRatings.Queries.GetRatingsByUserId;

public class GetMasterRatingsByUserIdHandler : IRequestHandler<GetMasterRatingsByUserIdQuery, List<MasterRatingResponseDTO>>
{
    private readonly IMasterRatingRepository _ratingRepository;
    private readonly IMapper _mapper;

    public GetMasterRatingsByUserIdHandler(IMasterRatingRepository ratingRepository, IMapper mapper)
    {
        _ratingRepository = ratingRepository;
        _mapper = mapper;
    }

    public async Task<List<MasterRatingResponseDTO>> Handle(GetMasterRatingsByUserIdQuery request, CancellationToken ct)
    {
        var ratings = await _ratingRepository.GetByMasterIdAsync(request.MasterId, ct);
        
        return _mapper.Map<List<MasterRatingResponseDTO>>(ratings);
    }
}
