using BlogSystem.Domain.Entities;
using Bogus;

namespace BlogSystem.UnitTests.Common.Builders;

public class AuthorBuilder
{
    private readonly Author _author;
    private readonly Faker _faker = new();

    public AuthorBuilder()
    {
        // NOTE: id is created automatically when we create new author - Look at Base Entity class
        _author = new Author(
            name: _faker.Person.FullName,
            email: _faker.Person.Email,
            imageUrl: _faker.Internet.Avatar()
        );
    }

    public AuthorBuilder WithName(string name)
    {
        _author.UpdateDetails(name: name);
        return this;
    }

    public AuthorBuilder WithEmail(string email)
    {
        _author.UpdateDetails(email: email);
        return this;
    }

    public AuthorBuilder WithImageUrl(string imageUrl)
    {
        _author.UpdateDetails(imageUrl: imageUrl);
        return this;
    }

    public AuthorBuilder WithBlogs(List<Blog> blogs)
    {
        _author.UpdateDetails(blogs: blogs);
        return this;
    }

    public Author Build() => _author;

    public static implicit operator Author(AuthorBuilder builder) => builder.Build();
}
