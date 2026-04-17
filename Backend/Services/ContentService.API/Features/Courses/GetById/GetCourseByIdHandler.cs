using ContentService.API.Data;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContentService.API.Features.Courses.GetById;

public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, Result<CourseDetailsDto>>
{
    private readonly ContentDbContext _context;

    public GetCourseByIdHandler(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CourseDetailsDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Lessons.OrderBy(l => l.Order))
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
            return Result.Fail<CourseDetailsDto>($"Course with id '{request.Id}' was not found.");

        var lessons = course.Lessons
            .Select(l => new CourseLessonDto(
                l.Id,
                l.Title,
                l.Order,
                l.VideoFileId.HasValue))
            .ToList();

        var dto = new CourseDetailsDto(
            course.Id,
            course.Title,
            course.Description,
            course.CreatedAt,
            course.UpdatedAt,
            lessons);

        return Result.Ok(dto);
    }
}
