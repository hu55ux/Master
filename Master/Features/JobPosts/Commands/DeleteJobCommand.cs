using MediatR;

namespace Master.Features.JobPosts.Commands;

public record DeleteJobCommand(Guid JobId, Guid ClientId) : IRequest<bool>;