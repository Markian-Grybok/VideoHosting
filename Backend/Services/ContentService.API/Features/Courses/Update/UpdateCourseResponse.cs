namespace ContentService.API.Features.Courses.Update;

public record UpdateCourseResponse(Guid Id, string Title, string Description, DateTime UpdatedAt);
