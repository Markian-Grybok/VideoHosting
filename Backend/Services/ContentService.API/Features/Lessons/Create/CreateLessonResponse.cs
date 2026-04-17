namespace ContentService.API.Features.Lessons.Create;

public record CreateLessonResponse(
    Guid Id,
    string Title,
    string Description,
    Guid CourseId,
    int Order,
    Guid? VideoFileId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
