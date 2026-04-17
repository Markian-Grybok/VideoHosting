using ContentService.API.Data;
using ContentService.API.Infrastructure.Http;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContentService.API.Features.Lessons.GetById;

public class GetLessonByIdHandler : IRequestHandler<GetLessonByIdQuery, Result<LessonDetailsDto>>
{
    private readonly ContentDbContext _context;
    private readonly IFileServiceClient _fileServiceClient;

    public GetLessonByIdHandler(ContentDbContext context, IFileServiceClient fileServiceClient)
    {
        _context = context;
        _fileServiceClient = fileServiceClient;
    }

    public async Task<Result<LessonDetailsDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (lesson is null)
            return Result.Fail<LessonDetailsDto>($"Lesson with id '{request.Id}' was not found.");

        // Fetch video info from FileService (optional — graceful degradation)
        string? playbackUrl = null;
        string? videoStatus = null;

        if (lesson.VideoFileId.HasValue)
        {
            var fileStatus = await _fileServiceClient
                .GetFileStatusAsync(lesson.VideoFileId.Value, cancellationToken);

            videoStatus = fileStatus?.Status;

            if (fileStatus?.Status == "Ready")
            {
                var playback = await _fileServiceClient
                    .GetPlaybackUrlAsync(lesson.VideoFileId.Value, cancellationToken);
                playbackUrl = playback?.Url;
            }
        }

        var dto = new LessonDetailsDto(
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.CourseId,
            lesson.Order,
            lesson.VideoFileId,
            playbackUrl,
            videoStatus,
            lesson.CreatedAt,
            lesson.UpdatedAt);

        return Result.Ok(dto);
    }
}
