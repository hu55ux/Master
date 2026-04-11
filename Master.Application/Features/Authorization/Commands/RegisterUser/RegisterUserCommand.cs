using Master.Application.DTOs;
using MediatR;
namespace Master.Application.Features.Authorization.Commands.RegisterUser;

/// <summary>
/// Command for registering a new user in the system.
/// </summary>
/// <param name="Request">The registration data including details like email, password, and personal info.</param>
public record RegisterUserCommand(RegisterRequest Request) : IRequest<AuthResponseDTO>;