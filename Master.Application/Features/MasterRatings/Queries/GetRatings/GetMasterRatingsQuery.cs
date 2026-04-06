using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.MasterRatings.Queries.GetRatings;

public record GetMasterRatingsQuery(Guid MasterId) : IRequest<List<MasterRatingResponseDTO>>;
