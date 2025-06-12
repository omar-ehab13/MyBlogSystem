using BlogSystem.Application.Features.Authors.Commands.UpdateAuthorCommand;
using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Exceptions;
using BlogSystem.Domain.Repositories;
using BlogSystem.UnitTests.Common.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace BlogSystem.UnitTests.Application.Commands.Authors;

public class UpdateAuthorCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMappingService> _mockMapper;
    private readonly UpdateAuthorCommandHandler _handler;
    private readonly Mock<IAuthorRepository> _mockAuthorRepository;

    public UpdateAuthorCommandHandlerTests()
    {
        _mockUnitOfWork = MockUnitOfWork.Create();
        _mockMapper = new Mock<IMappingService>();
        _mockAuthorRepository = new Mock<IAuthorRepository>();

        _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
            .Returns(_mockAuthorRepository.Object);

        _handler = new UpdateAuthorCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_AuthorNotFound_ShuouldThrowNotFoundException()
    {
        #region Arrange
        var updateAuthorDto = new UpdateAuthorDto(
            Name: "Omar Ehab",
            Email: "omar@gmail.com",
            ImageUrl: "https://example.com/image.png");

        var command = new UpdateAuthorCommand(Guid.NewGuid(), updateAuthorDto);

        // Setups
        // 1. setup authorRepo.GetById(id)
        _mockAuthorRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        #endregion

        #region Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

        exception.Should().NotBeNull();
        exception.Should().BeOfType<NotFoundException>();
        exception.Message.Should().Be($"The author id: {command.Id} not found");

        _mockAuthorRepository.Verify(x => x.GetByIdAsync(command.Id, CancellationToken.None), Times.Once());
        #endregion
    }

    [Fact]
    public async Task Handle_NewEmailIsExists_ShouldReturnFailureResult()
    {
        #region Arrange
        var updateAuthorDto = new UpdateAuthorDto(
            Name: "Omar Ehab",
            Email: "omar@gmail.com",
            ImageUrl: "https://example.com/image.png");

        var command = new UpdateAuthorCommand(Guid.NewGuid(), updateAuthorDto);
        var targetAuthor = new Author(
            name: command.AuthorDto.Name,
            email: command.AuthorDto.Email,
            imageUrl: command.AuthorDto.ImageUrl);

        // Setups
        // 1. setup authorRepo.GetById(id)
        _mockAuthorRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetAuthor);

        // 2. IsEmailExists
        _mockAuthorRepository.Setup(x => x.EmailExistsAsync(command.AuthorDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        #endregion

        #region Act
        var result = await _handler.Handle(command, CancellationToken.None);
        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Author with this email already exists");
        result.StatusCode.Should().Be(409);

        _mockAuthorRepository.Verify(x => x.GetByIdAsync(command.Id, CancellationToken.None), Times.Once());
        _mockAuthorRepository.Verify(x => x.EmailExistsAsync(command.AuthorDto.Email, It.IsAny<CancellationToken>()), Times.Once);
        #endregion
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSuccessResult()
    {
        #region Arrange
        var updateAuthorDto = new UpdateAuthorDto(
            Name: "Omar Ehab",
            Email: "omar@gmail.com",
            ImageUrl: "https://example.com/image.png");

        var command = new UpdateAuthorCommand(Guid.NewGuid(), updateAuthorDto);
        var targetAuthor = new Author(
            name: command.AuthorDto.Name,
            email: command.AuthorDto.Email,
            imageUrl: command.AuthorDto.ImageUrl);

        var authorDto = new AuthorDto(
                Id: targetAuthor.Id,
                Name: targetAuthor.Name,
                Email: targetAuthor.Email,
                ImageUrl: targetAuthor.ImageUrl,
                CreatedAt: targetAuthor.CreatedAt,
                UpdatedAt: targetAuthor.UpdatedAt
            );

        // Setups
        // 1. setup authorRepo.GetById(id)
        _mockAuthorRepository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetAuthor);

        // 2. IsEmailExists
        _mockAuthorRepository.Setup(x => x.EmailExistsAsync(command.AuthorDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // 3. Setup mapping from author to authorDto
        _mockMapper.Setup(x => x.Map<AuthorDto>(targetAuthor))
            .Returns(authorDto);
        #endregion

        #region Act
        var result = await _handler.Handle(command, CancellationToken.None);
        #endregion

        #region Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().Be(authorDto);
        result.Message.Should().Be("Updated");
        result.StatusCode.Should().Be(204);

        _mockAuthorRepository.Verify(x => x.GetByIdAsync(command.Id, CancellationToken.None), Times.Once());
        _mockAuthorRepository.Verify(x => x.EmailExistsAsync(command.AuthorDto.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(x => x.Map<AuthorDto>(targetAuthor), Times.Once);
        #endregion
    }
}
