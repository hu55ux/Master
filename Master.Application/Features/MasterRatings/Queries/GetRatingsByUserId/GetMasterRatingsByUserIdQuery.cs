using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.MasterRatings.Queries.GetRatingsByUserId;

public record GetMasterRatingsByUserIdQuery(Guid MasterId) : IRequest<List<MasterRatingResponseDTO>>;
