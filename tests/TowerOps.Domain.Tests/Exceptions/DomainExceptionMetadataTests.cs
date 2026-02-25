using FluentAssertions;
using TowerOps.Domain.Exceptions;
using Xunit;

namespace TowerOps.Domain.Tests.Exceptions;

public class DomainExceptionMetadataTests
{
    [Fact]
    public void EntityNotFoundException_ShouldExposeLocalizationMetadata()
    {
        var ex = new EntityNotFoundException("WorkOrder", 42);

        ex.MessageKey.Should().Be("Error.EntityNotFound");
        ex.MessageArguments.Should().ContainInOrder("WorkOrder", 42);
    }

    [Fact]
    public void InvalidStateTransitionException_ShouldExposeLocalizationMetadata()
    {
        var ex = new InvalidStateTransitionException("Created", "Closed");

        ex.MessageKey.Should().Be("Error.InvalidStateTransition");
        ex.MessageArguments.Should().ContainInOrder("Created", "Closed");
    }
}
