using Master.Application.DTOs;
using MediatR;

namespace Master.Application.Features.Skills.Queries.GetById;

public record GetByIdQuery(Guid Id) : IRequest<SkillResponseDTO>;
