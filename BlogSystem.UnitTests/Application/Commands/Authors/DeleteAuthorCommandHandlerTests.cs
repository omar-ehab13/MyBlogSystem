using BlogSystem.Application.Features.Authors.Commands.DeleteAuthorCommand;
using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Exceptions;
using BlogSystem.Domain.Repositories;
using BlogSystem.UnitTests.Common.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogSystem.UnitTests.Application.Commands.Authors;

public class DeleteAuthorCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly DeleteAuthorCommandHandler _handler;
    private readonly Mock<IAuthorRepository> _mockAuthorRepository;
    private readonly Mock<IMappingService> _mockMapper;

    public DeleteAuthorCommandHandlerTests()
    {
        _mockUnitOfWork = MockUnitOfWork.Create();
        _mockAuthorRepository = new Mock<IAuthorRepository>();
        _mockMapper = new Mock<IMappingService>();

        _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
            .Returns(_mockAuthorRepository.Object);

        _handler = new DeleteAuthorCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region Private Helper Methods for Arrange

    private DeleteAuthorCommand CreateDeleteCommand(Guid? id = null)
    {
        return new DeleteAuthorCommand(id ?? Guid.NewGuid());
    }

    private Author CreateTestAuthor(string name = "Test Author", string email = "test@example.com", string? imageUrl = null)
    {
        var author = new Author(name, email, imageUrl);

        return author;
    }

    private void SetupGetByIdAsync(Guid id, Author? authorToReturn)
    {
        _mockAuthorRepository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorToReturn);
    }

    private void SetupDelete(Author author)
    {
        _mockAuthorRepository.Setup(x => x.DeleteAsync(author, It.IsAny<CancellationToken>()));
    }

    private void SetupSaveChangesAsync(int affectedRows = 1)
    {
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(affectedRows);
    }
    #endregion

    [Fact]
    public async Task Handle_AuthorNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = CreateDeleteCommand(Guid.NewGuid());
        var author = CreateTestAuthor();

        SetupGetByIdAsync(command.Id, null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

        exception.Should().NotBeNull();
        exception.Should().BeOfType<NotFoundException>();
        exception.Message.Should().Be("The author already not found in DB");

        // Verify interactions
        _mockAuthorRepository.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _mockAuthorRepository.Verify(x => x.DeleteAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task Handle_SuccessfulDeletion_ShouldReturnSuccessResult()
    {
        // Arrange
        var command = CreateDeleteCommand();
        var existingAuthor = CreateTestAuthor();

        SetupGetByIdAsync(command.Id, existingAuthor);
        SetupDelete(existingAuthor);
        SetupSaveChangesAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Message.Should().Be("Deleted");
        result.StatusCode.Should().Be(204);

        // Verify interactions
        _mockAuthorRepository.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _mockAuthorRepository.Verify(x => x.DeleteAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}