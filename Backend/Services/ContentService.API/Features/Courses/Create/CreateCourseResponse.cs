namespace ContentService.API.Features.Courses.Create;

public record CreateCourseResponse(Guid Id, string Title, string Description, DateTime CreatedAt);
