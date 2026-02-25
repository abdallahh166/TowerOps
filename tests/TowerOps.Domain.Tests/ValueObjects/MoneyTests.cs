using FluentAssertions;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldCreateMoney()
    {
        // Act
        var money = Money.Create(100.50m, "EGP");

        // Assert
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("EGP");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Act
        var act = () => Money.Create(-10m, "EGP");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*negative*");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnSum()
    {
        // Arrange
        var money1 = Money.Create(100m, "EGP");
        var money2 = Money.Create(50m, "EGP");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("EGP");
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Create(100m, "EGP");
        var money2 = Money.Create(50m, "USD");

        // Act
        var act = () => money1.Add(money2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldReturnDifference()
    {
        // Arrange
        var money1 = Money.Create(100m, "EGP");
        var money2 = Money.Create(30m, "EGP");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70m);
        result.Currency.Should().Be("EGP");
    }

    [Fact]
    public void Subtract_ResultingInNegative_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Create(50m, "EGP");
        var money2 = Money.Create(100m, "EGP");

        // Act
        var act = () => money1.Subtract(money2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*negative*");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var money1 = Money.Create(100m, "EGP");
        var money2 = Money.Create(100m, "EGP");

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(100m, "EGP");
        var money2 = Money.Create(100m, "USD");

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }
}