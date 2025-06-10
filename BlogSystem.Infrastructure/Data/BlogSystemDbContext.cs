using BlogSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.Infrastructure.Data;

public class BlogSystemDbContext : DbContext
{
    public BlogSystemDbContext(DbContextOptions<BlogSystemDbContext> options) : base(options) { }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogSystemDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
