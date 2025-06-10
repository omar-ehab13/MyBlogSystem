using BlogSystem.Domain.Common;

namespace BlogSystem.Domain.Entities;

public class Author : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? ImageUrl { get; private set; }

    private readonly List<Blog> _blogs = new();
    public IReadOnlyCollection<Blog> Blogs => _blogs.AsReadOnly();

    private Author() { }

    public Author(string name, string email, string? imageUrl = null)
    {
        Name = name;
        Email = email;
        ImageUrl = imageUrl;
    }

    public void UpdateDetails(string name, string email, string? imageUrl = null)
    {
        Name = name;
        Email = email;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}