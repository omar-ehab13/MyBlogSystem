using BlogSystem.Application.Features.Authors.Queries;
using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Repositories;
using BlogSystem.UnitTests.Common.Helpers;
using BlogSystem.UnitTests.Common.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogSystem.UnitTests.Application.Queries.Authors;

public class GetAllAuthorsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMappingService> _mockMapper;
    private readonly GetAllAutorsQueryHandler _handler;
    private readonly Mock<IAuthorRepository> _mockAuthorRepository;

    public GetAllAuthorsQueryHandlerTests()
    {
        _mockUnitOfWork = MockUnitOfWork.Create();
        _mockMapper = new Mock<IMappingService>();
        _mockAuthorRepository = new Mock<IAuthorRepository>();

        _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
            .Returns(_mockAuthorRepository.Object);

        _handler = new GetAllAutorsQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region Private Helper Methods for Arrange

    private GetAllAuthorsQuery CreateGetAllAuthorsQuery()
    {
        return new GetAllAuthorsQuery();
    }

    private AuthorDto CreateTestAuthorDto(Author author)
    {
        return new AuthorDto(author.Id, author.Name, author.Email, author.ImageUrl, author.CreatedAt, author.UpdatedAt);
    }

    private void SetupGetAllAsync(IEnumerable<Author> authorsToReturn)
    {
        _mockAuthorRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorsToReturn);
    }

    private void SetupMapListAuthorsToDto(IEnumerable<Author> authors, List<AuthorDto> dtos)
    {
        _mockMapper.Setup(x => x.Map<List<AuthorDto>>(authors))
            .Returns(dtos);
    }
    #endregion

    [Fact]
    public async Task Handle_AuthorsExist_ShouldReturnSuccessResultWithAuthors()
    {
        // Arrange
        var query = CreateGetAllAuthorsQuery();

        var authors = TestDataGenerator.GenereateAuthors();

        var authorDto1 = CreateTestAuthorDto(authors[0]);
        var authorDto2 = CreateTestAuthorDto(authors[1]);
        var authorDtos = new List<AuthorDto> { authorDto1, authorDto2 };

        SetupGetAllAsync(authors);
        SetupMapListAuthorsToDto(authors, authorDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().BeEquivalentTo(authorDtos);
        result.Message.Should().Be("Authors retrieved successfully.");
        result.StatusCode.Should().Be(200);

        // Verify interactions
        _mockAuthorRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _mockMapper.Verify(x => x.Map<List<AuthorDto>>(authors), Times.Once);
    }

    [Fact]
    public async Task Handle_NoAuthorsExist_ShouldReturnSuccessResultWithEmptyList()
    {
        // Arrange
        var query = CreateGetAllAuthorsQuery();
        var authors = new List<Author>();
        var authorDtos = new List<AuthorDto>();

        SetupGetAllAsync(authors);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(authorDtos);
        result.Data.Should().BeEmpty();
        result.Message.Should().Be("No authors found");
        result.StatusCode.Should().Be(200);

        // Verify interactions
        _mockAuthorRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(x => x.Map<List<AuthorDto>>(It.IsAny<IEnumerable<Author>>()), Times.Never);
    }
}
