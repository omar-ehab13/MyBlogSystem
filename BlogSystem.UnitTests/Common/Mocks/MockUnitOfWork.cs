using BlogSystem.Domain.Repositories;
using Moq;

namespace BlogSystem.UnitTests.Common.Mocks;

public static class MockUnitOfWork
{
    public static Mock<IUnitOfWork> Create()
    {
        var mock = new Mock<IUnitOfWork>();

        // setup default behaviors
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // setup repositories mock
        var authorRepoMock = new Mock<IAuthorRepository>();
        var blogRepoMock = new Mock<IBlogRepository>();

        mock.Setup(x => x.AuthorRepository).Returns(authorRepoMock.Object);
        mock.Setup(x => x.BlogRepository).Returns(blogRepoMock.Object);

        return mock;
    }
}
