using FluentResults;
using MediatR;

namespace ContentService.API.Features.Lessons.GetById;

public record GetLessonByIdQuery(Guid Id) : IRequest<Result<LessonDetailsDto>>;
