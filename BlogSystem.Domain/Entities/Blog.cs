using BlogSystem.Domain.Common;

namespace BlogSystem.Domain.Entities;

public class Blog : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public Guid AuthorId { get; private set; }
    public Author Author { get; private set; } = null!;

    private Blog() { }

    public Blog(string title, string body, Guid authorId)
    {
        Title = title;
        Body = body;
        AuthorId = authorId;
    }

    public void Update(string title, string body)
    {
        Title = title;
        Body = body;
        UpdatedAt = DateTime.UtcNow;
    }
}