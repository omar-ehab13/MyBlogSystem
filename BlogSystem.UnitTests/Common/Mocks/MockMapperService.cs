using BlogSystem.Application.Interfaces;
using Moq;

namespace BlogSystem.UnitTests.Common.Mocks;

public static class MockMapperService
{
    public static Mock<IMappingService> Create()
    {
        var mock = new Mock<IMappingService>();

        // Setup default mapping behavior
        mock.Setup(x => x.Map<It.IsAnyType>(It.IsAny<object>()))
            .Returns<object, Type>((source, type) =>
            {
                try
                {
                    return (dynamic)source;
                }
                catch
                {
                    return default!;
                }
            });

        return mock;
    }
}
