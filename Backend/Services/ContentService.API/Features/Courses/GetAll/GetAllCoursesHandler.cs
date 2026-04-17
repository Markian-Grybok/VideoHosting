using ContentService.API.Data;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContentService.API.Features.Courses.GetAll;

public class GetAllCoursesHandler : IRequestHandler<GetAllCoursesQuery, Result<List<CourseDto>>>
{
    private readonly ContentDbContext _context;

    public GetAllCoursesHandler(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CourseDto>>> Handle(
        GetAllCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var courses = await _context.Courses
            .Include(c => c.Lessons)
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CourseDto(
                c.Id,
                c.Title,
                c.Description,
                c.Lessons.Count,
                c.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Ok(courses);
    }
}
