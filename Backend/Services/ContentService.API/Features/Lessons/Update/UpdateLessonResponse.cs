namespace ContentService.API.Features.Lessons.Update;

public record UpdateLessonResponse(
    Guid Id,
    string Title,
    string Description,
    Guid CourseId,
    int Order,
    Guid? VideoFileId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
