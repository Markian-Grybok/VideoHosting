using FluentResults;
using MediatR;

namespace ContentService.API.Features.Lessons.Delete;

public record DeleteLessonCommand(Guid Id) : IRequest<Result>;
