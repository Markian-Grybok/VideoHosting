using ContentService.API.Data;
using ContentService.API.Models;
using FluentResults;
using MediatR;

namespace ContentService.API.Features.Lessons.Create;

public class CreateLessonHandler : IRequestHandler<CreateLessonCommand, Result<CreateLessonResponse>>
{
    private readonly ContentDbContext _context;

    public CreateLessonHandler(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateLessonResponse>> Handle(
        CreateLessonCommand request,
        CancellationToken cancellationToken)
    {
        var lesson = Lesson.Create(
            request.Title,
            request.Description,
            request.CourseId,
            request.Order,
            request.VideoFileId);

        await _context.Lessons.AddAsync(lesson, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new CreateLessonResponse(
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.CourseId,
            lesson.Order,
            lesson.VideoFileId,
            lesson.CreatedAt,
            lesson.UpdatedAt);

        return Result.Ok(response);
    }
}
