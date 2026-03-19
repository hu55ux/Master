using Master.Application.Interfaces;
using Master.Application.Models;
using MediatR;

namespace Master.Application.Features.Authorization.Queries.GetUserEntity;

public class GetUserEntityHandler : IRequestHandler<GetUserEntityQuery, AppUser>
{
    private readonly IAuthRepository _authRepository;

    public GetUserEntityHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<AppUser> Handle(GetUserEntityQuery query, CancellationToken ct)
    {
        var user = await _authRepository.GetUserWithDetailsAsync(query.UserId, ct);
        return user ?? throw new KeyNotFoundException("User not found.");
    }
}