using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.MasterStatusFeature.Queries.GetMasterStatus;

/// <summary>
/// Query to retrieve the current availability status of a master.
/// </summary>
public record GetMasterStatusQuery(Guid MasterId) : IRequest<MasterStatusResponseDTO>;
