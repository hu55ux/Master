using Master.Application.DTOs;
using Master.Domain.Enums;
using MediatR;

namespace Master.Application.Features.MasterStatusFeature.Queries.GetAllMasterStatuses;

/// <summary>
/// Query to list all master status options for lookup dropdowns.
/// </summary>
public record GetAllMasterStatusesQuery : IRequest<IEnumerable<MasterStatusLookupDto>>;

/// <summary>
/// Handler to list all master status options.
/// </summary>
public class GetAllMasterStatusesHandler : IRequestHandler<GetAllMasterStatusesQuery, IEnumerable<MasterStatusLookupDto>>
{
    public Task<IEnumerable<MasterStatusLookupDto>> Handle(GetAllMasterStatusesQuery request, CancellationToken cancellationToken)
    {
        var statuses = MasterStatus.GetAll()
            .Select(s => new MasterStatusLookupDto(s.Id, s.Name, s.DisplayName, s.ColorCode, s.CanAcceptJobs));

        return Task.FromResult(statuses);
    }
}
