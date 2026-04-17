namespace ContentService.API.Models;

public class Course
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ICollection<Lesson> Lessons { get; private set; } = new List<Lesson>();

    private Course() { }

    public static Course Create(string title, string description) => new Course
    {
        Id = Guid.NewGuid(),
        Title = title,
        Description = description,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public void Update(string title, string description)
    {
        Title = title;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}
