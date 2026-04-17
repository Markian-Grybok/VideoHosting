namespace ContentService.API.Features.Lessons.Create;

public record CreateLessonRequest(
    string Title,
    string Description,
    Guid CourseId,
    int Order,
    Guid? VideoFileId = null);
