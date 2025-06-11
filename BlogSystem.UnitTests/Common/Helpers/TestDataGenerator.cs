using BlogSystem.Domain.Entities;
using BlogSystem.UnitTests.Common.Builders;

namespace BlogSystem.UnitTests.Common.Helpers;

public static class TestDataGenerator
{
    private static readonly Random random = new Random();

    public static List<Author> GenereateAuthors(int count = 5)
    {
        return Enumerable
                .Range(1, count)
                .Select(i => new AuthorBuilder().Build())
                .ToList();
    }

    public static List<Blog> GenerateBlogs(int count = 10, List<Guid>? authorIds = null)
    {
        // If no author IDs are provided, generate a default set of authorIds
        if (authorIds == null || !authorIds.Any())
        {
            authorIds = GenereateAuthors(5)
                        .Select(author => author.Id)
                        .ToList();
        }

        return Enumerable
                .Range(1, count)
                .Select(i =>
                {
                    var randomAuthorId = authorIds[random.Next(authorIds.Count)];

                    return new BlogBuilder()
                              .WithAuthorId(randomAuthorId)
                              .Build();
                })
                .ToList();
    }

    public static Author GenerateAuthorWithBlogs(int blogCount = 3)
    {
        var author = new AuthorBuilder().Build();
        var blogs = GenerateBlogs(blogCount, new List<Guid> {author.Id});

        return new AuthorBuilder()
            .WithName(author.Name)
            .WithEmail(author.Email)
            .WithBlogs(blogs)
            .Build();
    }
}
