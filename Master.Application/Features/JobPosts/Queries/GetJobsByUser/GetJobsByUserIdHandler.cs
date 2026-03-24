using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.JobPosts.Queries.GetJobsByUser;

public class GetJobsByUserIdHandler : IRequestHandler<GetJobsByUserIdQuery, IEnumerable<JobPostResponseDTO>>
{
    private readonly IJobPostRepository _repo;
    private readonly IMapper _mapper;


    public GetJobsByUserIdHandler(IJobPostRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JobPostResponseDTO>> Handle(GetJobsByUserIdQuery request, CancellationToken ct)
    {
        var jobs = await _repo.GetJobsByUserIdAsync(request.UserId, !request.IsOwner, ct);

        return _mapper.Map<IEnumerable<JobPostResponseDTO>>(jobs);
    }
}