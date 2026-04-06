using MediatR;

namespace Master.Application.Features.MasterRatings.Commands.DeleteRating;

public record DeleteMasterRatingCommand(Guid MasterId, Guid CustomerId) : IRequest<bool>;
