using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Master.Application.Features.JobPosts.Queries.GetUserByJob;

public class GetUserByJobIdHandler : IRequestHandler<GetUserByJobIdQuery, AuthResponseDTO>
{
    private readonly IJobPostRepository _repo;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public GetUserByJobIdHandler(IJobPostRepository repo, IMapper mapper, UserManager<AppUser> userManager)
    {
        _repo = repo;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<AuthResponseDTO> Handle(GetUserByJobIdQuery request, CancellationToken ct)
    {
        var customer = await _repo.GetCustomerByJobIdAsync(request.JobId, ct);

        if (customer == null)
            throw new KeyNotFoundException("Bu iş elanına bağlı müştəri tapılmadı.");

        var response = _mapper.Map<AuthResponseDTO>(customer);

        var roles = await _userManager.GetRolesAsync(customer);
        response.Roles = roles.ToList();

        return response;
    }
}