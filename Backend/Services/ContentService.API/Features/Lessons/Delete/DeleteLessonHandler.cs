using ContentService.API.Data;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContentService.API.Features.Lessons.Delete;

public class DeleteLessonHandler : IRequestHandler<DeleteLessonCommand, Result>
{
    private readonly ContentDbContext _context;

    public DeleteLessonHandler(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteLessonCommand request,
        CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (lesson is null)
            return Result.Fail($"Lesson with id '{request.Id}' was not found.");

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
