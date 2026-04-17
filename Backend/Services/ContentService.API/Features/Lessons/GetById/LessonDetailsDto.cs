namespace ContentService.API.Features.Lessons.GetById;

public record LessonDetailsDto(
    Guid Id,
    string Title,
    string Description,
    Guid CourseId,
    int Order,
    Guid? VideoFileId,
    string? PlaybackUrl,
    string? VideoStatus,
    DateTime CreatedAt,
    DateTime UpdatedAt);
