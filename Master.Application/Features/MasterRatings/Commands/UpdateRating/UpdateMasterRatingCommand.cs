using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.MasterRatings.Commands.UpdateRating;

public record UpdateMasterRatingCommand(UpdateMasterRatingDTO Model) : IRequest<bool>;
