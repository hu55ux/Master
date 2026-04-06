using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Master.Application.Features.MasterRatings.Commands.UpdateRating;

public class UpdateMasterRatingHandler : IRequestHandler<UpdateMasterRatingCommand, bool>
{
    private readonly IMasterRatingRepository _ratingRepository;

    public UpdateMasterRatingHandler(
        IMasterRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
    }

    public async Task<bool> Handle(UpdateMasterRatingCommand request, CancellationToken ct)
    {
        var dto = request.Model;

        var rating = await _ratingRepository.GetAsync(dto.MasterId, dto.CustomerId, ct);
        if (rating == null)
            throw new KeyNotFoundException("Rating not found.");

        // 1. Update the rating entry (tracked by DbContext)
        rating.Score = dto.Score;
        rating.Comment = dto.Comment;

        // Save first so DB has the new state for UpdateMasterStatsAsync
        await _ratingRepository.SaveChangesAsync(ct);

        // 2. Update master's aggregate statistics using Repository logic (as requested)
        await _ratingRepository.UpdateMasterStatsAsync(dto.MasterId, ct);

        // 3. Save atomically.
        await _ratingRepository.SaveChangesAsync(ct);

        return true;
    }
}
