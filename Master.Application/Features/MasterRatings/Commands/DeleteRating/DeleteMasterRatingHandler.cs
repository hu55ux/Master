using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.MasterRatings.Commands.DeleteRating;

public class DeleteMasterRatingHandler : IRequestHandler<DeleteMasterRatingCommand, bool>
{
    private readonly IMasterRatingRepository _ratingRepository;

    public DeleteMasterRatingHandler(IMasterRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async Task<bool> Handle(DeleteMasterRatingCommand request, CancellationToken ct)
    {
        var rating = await _ratingRepository.GetAsync(request.MasterId, request.CustomerId, ct);
        if (rating == null)
            throw new KeyNotFoundException("Rating not found.");

        // 1. Delete the rating record
        await _ratingRepository.DeleteAsync(rating, ct);

        // Save first so DB has the new state for UpdateMasterStatsAsync
        await _ratingRepository.SaveChangesAsync(ct);

        // 2. Update master's aggregate statistics using Repository logic (as requested)
        await _ratingRepository.UpdateMasterStatsAsync(request.MasterId, ct);

        // 3. Save all changes atomically
        await _ratingRepository.SaveChangesAsync(ct);

        return true;
    }
}
