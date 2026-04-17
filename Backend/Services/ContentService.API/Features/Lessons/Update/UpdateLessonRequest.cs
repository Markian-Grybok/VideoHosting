namespace ContentService.API.Features.Lessons.Update;

public record UpdateLessonRequest(
    string Title, 
    string Description, 
    int Order, 
    Guid? VideoFileId = null);
