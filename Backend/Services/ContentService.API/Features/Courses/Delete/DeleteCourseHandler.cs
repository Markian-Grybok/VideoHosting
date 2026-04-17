using ContentService.API.Data;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContentService.API.Features.Courses.Delete;

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Result>
{
    private readonly ContentDbContext _context;

    public DeleteCourseHandler(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteCourseCommand request,
        CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
            return Result.Fail($"Course with id '{request.Id}' was not found.");

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
