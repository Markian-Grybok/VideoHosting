using FluentResults;
using MediatR;

namespace ContentService.API.Features.Lessons.GetAll;

public record GetAllLessonsQuery(Guid? CourseId = null) : IRequest<Result<IReadOnlyList<LessonDto>>>;
