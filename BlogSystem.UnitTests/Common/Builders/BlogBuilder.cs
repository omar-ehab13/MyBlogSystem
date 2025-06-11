using BlogSystem.Domain.Entities;
using Bogus;

namespace BlogSystem.UnitTests.Common.Builders;

public class BlogBuilder
{
    private readonly Blog _blog;
    private readonly Faker _faker = new();

    public BlogBuilder()
    {
        _blog = new Blog (
            title: _faker.Lorem.Sentence(),
            body: _faker.Lorem.Paragraphs(3),
            authorId: _faker.Random.Guid()
        );
    }

    public BlogBuilder WithTitle(string title)
    {
        _blog.Update(title: title);
        return this;
    }

    public BlogBuilder WithBody(string body)
    {
        _blog.Update(body: body);
        return this;
    }

    public BlogBuilder WithAuthorId(Guid authorId)
    {
        _blog.Update(authorId: authorId);
        return this;
    }

    public Blog Build() => _blog;

    public static implicit operator Blog(BlogBuilder builder) => builder.Build();
}
