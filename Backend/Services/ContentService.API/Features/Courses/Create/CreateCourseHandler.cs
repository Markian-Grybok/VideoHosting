using ContentService.API.Data;
using ContentService.API.Models;
using FluentResults;
using MediatR;

namespace ContentService.API.Features.Courses.Create;

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, Result<CreateCourseResponse>>
{
    private readonly ContentDbContext _context;

    public CreateCourseHandler(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateCourseResponse>> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken)
    {
        var course = Course.Create(request.Title, request.Description);

        await _context.Courses.AddAsync(course, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new CreateCourseResponse(
            course.Id,
            course.Title,
            course.Description,
            course.CreatedAt);

        return Result.Ok(response);
    }
}
