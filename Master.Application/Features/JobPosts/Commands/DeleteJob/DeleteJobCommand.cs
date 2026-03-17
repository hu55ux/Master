using MediatR;

namespace Master.Features.JobPosts.Commands.DeleteJob;

public record DeleteJobCommand(Guid JobId, Guid ClientId) : IRequest<bool>;