using AutoMapper;
using Master.Application.Common;
using Master.Application.DTOs;
using Master.Application.Interfaces;
using Master.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Master.Domain.Models;

namespace Master.Application.Features.Authorization.Queries.GetClientsList;

public class GetClientsListHandler : IRequestHandler<GetClientsListQuery, PagedResult<AuthResponseDTO>>
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public GetClientsListHandler(IAuthRepository authRepository, IMapper mapper, UserManager<AppUser> userManager)
    {
        _authRepository = authRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<PagedResult<AuthResponseDTO>> Handle(GetClientsListQuery request, CancellationToken cancellationToken)
    {
        var pagedResults = await _authRepository.GetUsersPagedAsync(
            UserRoles.Client, 
            request.Query.PageNumber, 
            request.Query.PageSize, 
            request.Query.Search, 
            "name");
        
        var dtos = new List<AuthResponseDTO>();
        
        foreach (var client in pagedResults.Items)
        {
            var dto = _mapper.Map<AuthResponseDTO>(client);
            var roles = await _userManager.GetRolesAsync(client);
            dto.Roles = roles.ToList();
            dtos.Add(dto);
        }

        return PagedResult<AuthResponseDTO>.Create(
            dtos, 
            pagedResults.Page, 
            pagedResults.PageSize, 
            pagedResults.TotalCount);
    }
}
