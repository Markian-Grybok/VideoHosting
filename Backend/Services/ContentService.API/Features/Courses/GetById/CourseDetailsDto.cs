namespace ContentService.API.Features.Courses.GetById;

public record CourseDetailsDto(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<CourseLessonDto> Lessons);

public record CourseLessonDto(Guid Id, string Title, int Order, bool HasVideo);
