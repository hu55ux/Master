using Master.Models;
using MediatR;

namespace Master.Features.JobPosts.Commands;

public record ChangeJobStatusCommand(Guid JobId, Guid ClientId, JobPostStatus NewStatus) : IRequest<bool>;
