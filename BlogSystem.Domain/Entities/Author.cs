using BlogSystem.Domain.Common;

namespace BlogSystem.Domain.Entities;

public class Author : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? ImageUrl { get; private set; }
    public List<Blog>? Blogs { get; set; } = new();

    private Author() { }

    public Author(string name, string email, string? imageUrl = null, List<Blog>? blogs = null)
    {
        Name = name;
        Email = email;
        ImageUrl = imageUrl;
        Blogs = blogs ?? new();
    }

    public void UpdateDetails(string? name = null, string? email = null, string? imageUrl = null, List<Blog>? blogs = null)
    {
        Name = name ?? Name;
        Email = email ?? Email;
        ImageUrl = imageUrl ?? ImageUrl;
        Blogs = blogs ?? Blogs;
        UpdatedAt = DateTime.UtcNow;
    }
}