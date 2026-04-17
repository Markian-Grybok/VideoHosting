namespace ContentService.API.Features.Courses.GetAll;

public record CourseDto(Guid Id, string Title, string Description, int LessonCount, DateTime CreatedAt);
