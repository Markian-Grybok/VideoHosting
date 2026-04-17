using FluentResults;
using MediatR;

namespace ContentService.API.Features.Lessons.Create;

public record CreateLessonCommand(
    string Title,
    string Description,
    Guid CourseId,
    int Order,
    Guid? VideoFileId)
    : IRequest<Result<CreateLessonResponse>>;
