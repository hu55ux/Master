using Master.Application.DTOs;
using Master.Domain.Enums;
using MediatR;

namespace Master.Application.Features.MasterStatusFeature.Commands.UpdateMasterStatus;

/// <summary>
/// Command to update the status of a master.
/// </summary>
public record UpdateMasterStatusCommand(Guid MasterId, MasterStatus Status) : IRequest<MasterStatusResponseDTO>;
