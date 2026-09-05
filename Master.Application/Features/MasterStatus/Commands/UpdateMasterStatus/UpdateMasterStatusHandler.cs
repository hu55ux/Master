using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Enums;
using MediatR;

namespace Master.Application.Features.MasterStatusFeature.Commands.UpdateMasterStatus;

/// <summary>
/// Handler for updating a master's availability status.
/// </summary>
public class UpdateMasterStatusHandler : IRequestHandler<UpdateMasterStatusCommand, MasterStatusResponseDTO>
{
    private readonly IAuthRepository _authRepository;

    public UpdateMasterStatusHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<MasterStatusResponseDTO> Handle(UpdateMasterStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetByIdAsync(request.MasterId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("Master user not found.");
        }

        user.Status = request.Status;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var result = await _authRepository.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update master status: {errors}");
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
