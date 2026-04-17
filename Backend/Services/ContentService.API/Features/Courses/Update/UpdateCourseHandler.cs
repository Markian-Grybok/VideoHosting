using ContentService.API.Data;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContentService.API.Features.Courses.Update;

public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Result<UpdateCourseResponse>>
{
    private readonly ContentDbContext _context;

    public UpdateCourseHandler(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UpdateCourseResponse>> Handle(
        UpdateCourseCommand request,
        CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
            return Result.Fail<UpdateCourseResponse>($"Course with id '{request.Id}' was not found.");

        course.Update(request.Title, request.Description);

        _context.Courses.Update(course);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new UpdateCourseResponse(
            course.Id,
            course.Title,
            course.Description,
            course.UpdatedAt);

        return Result.Ok(response);
    }
}
