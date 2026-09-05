using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.MasterStatusFeature.Queries.GetMasterStatus;

/// <summary>
/// Handler for retrieving a master's current availability status.
/// </summary>
public class GetMasterStatusHandler : IRequestHandler<GetMasterStatusQuery, MasterStatusResponseDTO>
{
    private readonly IAuthRepository _authRepository;

    public GetMasterStatusHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<MasterStatusResponseDTO> Handle(GetMasterStatusQuery request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetByIdAsync(request.MasterId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        var status = user.Status;

        return new MasterStatusResponseDTO
        {
            MasterId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            StatusId = status.Id,
            StatusName = status.Name,
            StatusDisplayName = status.DisplayName,
            ColorCode = status.ColorCode,
            CanAcceptJobs = status.CanAcceptJobs,
            UpdatedAt = user.UpdatedAt
        };
    }
}
