using Master.Application.Models;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.ChangejobStatus;

public record ChangeJobStatusCommand(Guid JobId, Guid ClientId, JobPostStatus NewStatus) : IRequest<bool>;
