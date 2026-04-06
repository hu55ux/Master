using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.MasterRatings.Commands.CreateRating;

public record CreateMasterRatingCommand(CreateMasterRatingDTO Model) : IRequest<bool>;
