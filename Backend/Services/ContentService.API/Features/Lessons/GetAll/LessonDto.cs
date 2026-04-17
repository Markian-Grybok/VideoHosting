namespace ContentService.API.Features.Lessons.GetAll;

public record LessonDto(
    Guid Id,
    string Title,
    string Description,
    Guid CourseId,
    int Order,
    Guid? VideoFileId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
