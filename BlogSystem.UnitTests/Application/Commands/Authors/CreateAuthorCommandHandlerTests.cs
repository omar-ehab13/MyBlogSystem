using BlogSystem.Application.Behaviors;
using BlogSystem.Application.Features.Authors.Commands.CreateAuthorCommand;
using BlogSystem.Application.Interfaces;
using BlogSystem.Domain.Common;
using BlogSystem.Domain.Entities;
using BlogSystem.Domain.Repositories;
using BlogSystem.UnitTests.Common.Builders;
using BlogSystem.UnitTests.Common.Mocks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace BlogSystem.UnitTests.Application.Commands.Authors;

public class CreateAuthorCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMappingService> _mockMapper;
    private readonly CreateAuthorCommandHandler _handler;
    private readonly Mock<IAuthorRepository> _mockAuthorRepository;

    public CreateAuthorCommandHandlerTests()
    {
        _mockUnitOfWork = MockUnitOfWork.Create();
        _mockMapper = new Mock<IMappingService>();
        _mockAuthorRepository = new Mock<IAuthorRepository>();

        _mockUnitOfWork.Setup(uow => uow.AuthorRepository)
            .Returns(_mockAuthorRepository.Object);

        _handler = new CreateAuthorCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSuccessResult()
    {
        // Arrange
        var createAuthorDto = new CreateAuthorDto(
            Name: "Omar Ehab",
            Email: "omarehab@example.com",
            ImageUrl: "https://example.com/image.jpg"
        );
        var command = new CreateAuthorCommand(createAuthorDto);

        var author = new AuthorBuilder()
            .WithName(command.AuthorDto.Name)
            .WithEmail(command.AuthorDto.Email)
            .WithImageUrl(command.AuthorDto.ImageUrl!)
            .Build();

        var authorDto = new AuthorDto(
            Id: author.Id,
            Name: author.Name,
            Email: author.Email,
            ImageUrl: author.ImageUrl,
            CreatedAt: author.CreatedAt,
            UpdatedAt: author.UpdatedAt
            );

        _mockMapper.Setup(x => x.Map<Author>(command.AuthorDto))
            .Returns(author);

        _mockMapper.Setup(x => x.Map<AuthorDto>(author))
            .Returns(authorDto);

        _mockAuthorRepository.Setup(x => x.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(author);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be(command.AuthorDto.Name);
        result.Data.Email.Should().Be(command.AuthorDto.Email);
        result.StatusCode.Should().Be(201);

        _mockAuthorRepository.Verify(x => x.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(x => x.Map<Author>(command.AuthorDto), Times.Once);
        _mockMapper.Verify(x => x.Map<AuthorDto>(author), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingEmail_ShouldReturnFailureResult()
    {
        // Arrange
        var createAuthorDto = new CreateAuthorDto(
            Name: "Omar Ehab",
            Email: "existing@example.com",
            ImageUrl: "https://example.com/image.jpg"
        );
        var command = new CreateAuthorCommand(createAuthorDto);

        // --- Mock Setups ---
        // EmailExistsAsync should return true (author already exists)
        _mockAuthorRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(true); // Simulate email exists

        // Act & Assert
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert on the exception details if needed

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.Message.Should().Be("Author with this email already exists");

        // Verify that only EmailExistsAsync was called, and nothing after the throw
        _mockAuthorRepository.Verify(x => x.EmailExistsAsync(command.AuthorDto.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(x => x.Map<Author>(It.IsAny<CreateAuthorDto>()), Times.Never);
        _mockAuthorRepository.Verify(x => x.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockAuthorRepository.Verify(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SaveChangesAsyncFails_ShouldReturnFailureResult()
    {
        // Arrange
        var createAuthorDto = new CreateAuthorDto(
            Name: "Omar Eahb",
            Email: "omar@example.com",
            ImageUrl: "https://example.com/image.jpg"
        );
        var command = new CreateAuthorCommand(createAuthorDto);

        var author = new AuthorBuilder().Build();

        _mockAuthorRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(false);

        _mockMapper.Setup(x => x.Map<Author>(command.AuthorDto))
            .Returns(author);

        _mockAuthorRepository.Setup(x => x.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(author);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(500);
        result.Message.Should().Be("Failed to retrieve created author.");
        result.Errors.Should().ContainSingle().And.Contain("Error: could not retrieve created author afeter saving");


        // Verify interactions up to the point of failure
        _mockAuthorRepository.Verify(x => x.EmailExistsAsync(command.AuthorDto.Email, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(x => x.Map<Author>(command.AuthorDto), Times.Once);
        _mockAuthorRepository.Verify(x => x.AddAsync(author, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Mapping to AuthorDto should NOT be called because createdAuthor is null
        _mockMapper.Verify(x => x.Map<AuthorDto>(It.IsAny<Author>()), Times.Never);
    }


}
