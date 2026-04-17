using ContentService.API.Data;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContentService.API.Features.Lessons.Update;

public class UpdateLessonHandler : IRequestHandler<UpdateLessonCommand, Result<UpdateLessonResponse>>
{
    private readonly ContentDbContext _context;

    public UpdateLessonHandler(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UpdateLessonResponse>> Handle(
        UpdateLessonCommand request,
        CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (lesson is null)
            return Result.Fail<UpdateLessonResponse>($"Lesson with id '{request.Id}' was not found.");

        lesson.Update(request.Title, request.Description, request.Order, request.VideoFileId);

        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new UpdateLessonResponse(
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
