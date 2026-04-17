namespace ContentService.API.Models;

public class Lesson
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid CourseId { get; private set; }
    public int Order { get; private set; }
    public Guid? VideoFileId { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    private Lesson() { }

    public static Lesson Create(string title, string description, Guid courseId, int order, Guid? videoFileId = null)
    {
        return new Lesson
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            CourseId = courseId,
            Order = order,
            VideoFileId = videoFileId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string title, string description, int order, Guid? videoFileId = null)
    {
        Title = title;
        Description = description;
        Order = order;
        VideoFileId = videoFileId;
        UpdatedAt = DateTime.UtcNow;
    }
}
