using ContentService.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ContentService.API.Data;

public class ContentDbContext : DbContext
{
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Course> Courses => Set<Course>();

    public ContentDbContext(DbContextOptions<ContentDbContext> options) 
        : base(options) 
    { 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
