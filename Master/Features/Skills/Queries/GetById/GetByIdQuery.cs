using Master.DTOs;
using MediatR;

namespace Master.Features.Skills.Queries.GetById;

public record GetByIdQuery(Guid Id) : IRequest<SkillResponseDTO>;
