using FluentAssertions;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Domain.Tests.ValueObjects;

public class VoltageReadingTests
{
    [Theory]
    [InlineData(180)]
    [InlineData(220)]
    [InlineData(240)]
    public void CreatePhaseToNeutral_WithValidVoltage_ShouldBeWithinRange(decimal voltage)
    {
        // Act
        var reading = VoltageReading.CreatePhaseToNeutral(voltage);

        // Assert
        reading.Value.Should().Be(voltage);
        reading.IsWithinRange.Should().BeTrue();
    }

    [Theory]
    [InlineData(170)]
    [InlineData(250)]
    public void CreatePhaseToNeutral_WithInvalidVoltage_ShouldNotBeWithinRange(decimal voltage)
    {
        // Act
        var reading = VoltageReading.CreatePhaseToNeutral(voltage);

        // Assert
        reading.IsWithinRange.Should().BeFalse();
    }

    [Theory]
    [InlineData(310)]
    [InlineData(380)]
    [InlineData(415)]
    public void CreatePhaseToPhase_WithValidVoltage_ShouldBeWithinRange(decimal voltage)
    {
        // Act
        var reading = VoltageReading.CreatePhaseToPhase(voltage);

        // Assert
        reading.IsWithinRange.Should().BeTrue();
    }

    [Fact]
    public void CreateDC_WithValidVoltage_ShouldBeWithinRange()
    {
        // Act
        var reading = VoltageReading.CreateDC(-54m);

        // Assert
        reading.Value.Should().Be(-54m);
        reading.IsWithinRange.Should().BeTrue();
    }

    [Theory]
    [InlineData(-50)]
    [InlineData(-58)]
    public void CreateDC_WithInvalidVoltage_ShouldNotBeWithinRange(decimal voltage)
    {
        // Act
        var reading = VoltageReading.CreateDC(voltage);

        // Assert
        reading.IsWithinRange.Should().BeFalse();
    }
}