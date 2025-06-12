using BlogSystem.Application.Features.Authors.Queries;
using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Exceptions;
using BlogSystem.Domain.Repositories;
using BlogSystem.UnitTests.Common.Builders;
using BlogSystem.UnitTests.Common.Helpers;
using BlogSystem.UnitTests.Common.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogSystem.UnitTests.Application.Queries.Authors;

public class GetAuthorByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMappingService> _mockMapper;
    private readonly GetAuthorByIdQueryHandler _handler;
    private readonly Mock<IAuthorRepository> _mockAuthorRepository;

    public GetAuthorByIdQueryHandlerTests()
    {
        _mockUnitOfWork = MockUnitOfWork.Create();
        _mockMapper = new Mock<IMappingService>();
        _mockAuthorRepository = new Mock<IAuthorRepository>();

        _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
            .Returns(_mockAuthorRepository.Object);

        _handler = new GetAuthorByIdQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region Private Helper Methods for Arrange

    private GetAuthorByIdQuery CreateAuthorByIdQuery(Guid id)
    {
        return new GetAuthorByIdQuery(id);
    }

    private Author CreateAuthor(string? name = "Omar Ehab", string? email = "omar@gmail.com", string? imageUrl = "https://example/imag.png")
    {
        return new AuthorBuilder()
            .WithName(name!)
            .WithEmail(email!)
            .WithImageUrl(imageUrl!)
            .Build();
    }

    private AuthorDto CreateTestAuthorDto(Author author)
    {
        return new AuthorDto(author.Id, author.Name, author.Email, author.ImageUrl, author.CreatedAt, author.UpdatedAt);
    }

    private void SetupGetAuthorById(Guid id, Author authorToReturn)
    {
        _mockAuthorRepository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorToReturn);
    }

    private void SetupMapAuthorToDto(Author author, AuthorDto authorDto)
    {
        _mockMapper.Setup(x => x.Map<AuthorDto>(author))
            .Returns(authorDto);
    }
    #endregion

    [Fact]
    public async Task Handle_AuthorByIdExist_ShouldReturnSuccessResultWithAuthors()
    {
        // Arrange
        var query = CreateAuthorByIdQuery(Guid.NewGuid());

        var author = CreateAuthor();

        var authorDto = CreateTestAuthorDto(author);

        SetupGetAuthorById(query.Id, author);
        SetupMapAuthorToDto(author, authorDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEquivalentTo(authorDto);
        result.Message.Should().Be("Author retrieved successfully");
        result.StatusCode.Should().Be(200);

        // Verify interactions
        _mockAuthorRepository.Verify(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once());
        _mockMapper.Verify(x => x.Map<AuthorDto>(author), Times.Once);
    }

    [Fact]
    public async Task Handle_AuthorNotFound_ShouldReturnSuccessResultWithEmptyList()
    {
        // Arrange
        var query = CreateAuthorByIdQuery(Guid.NewGuid());

        var author = CreateAuthor();

        SetupGetAuthorById(query.Id, null!);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () => await _handler.Handle(query, CancellationToken.None));

        // Assert
        exception.Should().NotBeNull();
        exception.Should().BeOfType<NotFoundException>();
        exception.Message.Should().Be("Author not found");

        // Verify interactions
        _mockAuthorRepository.Verify(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once());
        _mockMapper.Verify(x => x.Map<AuthorDto>(author), Times.Never);
    }

}