using BlogSystem.Domain.Common;
using FluentAssertions;

namespace BlogSystem.UnitTests.Common.Helpers;

public static class AssertionExtensions
{
    public static void ShouldBeSuccessResult<T>(this Result<T> result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Errors.Should().BeEmpty();
    }

    public static void ShouldBeFailureResult<T>(this Result<T> result, int expectedStatusCode = 400)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(expectedStatusCode);
        result.Errors.Should().NotBeEmpty();
    }

    public static void ShouldBeFailureResultWithError<T>(this Result<T> result, string expectedError, int expectedStatusCode = 400)
    {
        result.ShouldBeFailureResult(expectedStatusCode);
        result.Errors.Should().Contain(expectedError);
    }
}
