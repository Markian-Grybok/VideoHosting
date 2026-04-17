using ContentService.API.Data;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContentService.API.Features.Lessons.GetAll;

public class GetAllLessonsHandler : IRequestHandler<GetAllLessonsQuery, Result<IReadOnlyList<LessonDto>>>
{
    private readonly ContentDbContext _context;

    public GetAllLessonsHandler(ContentDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<LessonDto>>> Handle(
        GetAllLessonsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Lessons.AsNoTracking();

        // Filter by CourseId if provided
        if (request.CourseId.HasValue)
        {
            query = query.Where(l => l.CourseId == request.CourseId.Value);
        }

        var lessons = await query
            .OrderBy(l => l.Order)
            .Select(l => new LessonDto(
                l.Id,
                l.Title,
                l.Description,
                l.CourseId,
                l.Order,
                l.VideoFileId,
                l.CreatedAt,
                l.UpdatedAt))
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<LessonDto>>(lessons);
    }
}
