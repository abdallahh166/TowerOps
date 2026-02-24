using FluentAssertions;
using TelecomPM.Application.Exceptions;
using Xunit;

namespace TelecomPM.Application.Tests.Exceptions;

public class ApplicationExceptionMetadataTests
{
    [Fact]
    public void NotFoundException_ShouldExposeLocalizationMetadata()
    {
        var ex = new NotFoundException("Site", "ABC123");

        ex.MessageKey.Should().Be("Error.EntityNotFound");
        ex.MessageArguments.Should().ContainInOrder("Site", "ABC123");
    }

    [Fact]
    public void UnauthorizedAndConflict_ShouldExposeDefaultKeys()
    {
        var unauthorized = new UnauthorizedException("Denied.");
        var conflict = new ConflictException("Conflict.");

        unauthorized.MessageKey.Should().Be("Error.UnauthorizedGeneric");
        conflict.MessageKey.Should().Be("Error.ConflictGeneric");
    }
}
