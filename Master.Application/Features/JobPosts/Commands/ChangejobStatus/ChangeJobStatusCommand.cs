using Master.Domain.Models;
using MediatR;

namespace Master.Application.Features.JobPosts.Commands.ChangejobStatus;

public record ChangeJobStatusCommand(Guid JobId, Guid CustomerId, JobPostStatus NewStatus) : IRequest<bool>;
