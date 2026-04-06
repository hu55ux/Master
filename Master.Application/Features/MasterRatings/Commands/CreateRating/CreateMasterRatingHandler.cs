using AutoMapper;
using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;

namespace Master.Application.Features.MasterRatings.Commands.CreateRating;

public class CreateMasterRatingHandler : IRequestHandler<CreateMasterRatingCommand, bool>
{
    private readonly IMasterRatingRepository _ratingRepository;
    private readonly IMapper _mapper;

    public CreateMasterRatingHandler(
        IMasterRatingRepository ratingRepository,
        IMapper mapper)
    {
        _ratingRepository = ratingRepository;
        _mapper = mapper;
    }

    public async Task<bool> Handle(CreateMasterRatingCommand request, CancellationToken ct)
    {
        var dto = request.Model;

        var alreadyRated = await _ratingRepository.AlreadyRatedAsync(dto.MasterId, dto.CustomerId, ct);
        if (alreadyRated)
            throw new InvalidOperationException("User has already rated this master.");

        // 2. Create the rating record
        var rating = _mapper.Map<MasterRating>(dto);
        await _ratingRepository.AddAsync(rating, ct);

        // Save rating first so that UpdateMasterStatsAsync can query it from DB
        await _ratingRepository.SaveChangesAsync(ct);

        // 3. Update master's aggregate rating using Repository logic (as requested)
        await _ratingRepository.UpdateMasterStatsAsync(dto.MasterId, ct);

        // 4. Save everything in a single atomic operation.
        await _ratingRepository.SaveChangesAsync(ct);

        return true;
    }
}
