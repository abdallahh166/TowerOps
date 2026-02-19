using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required");

        return new Money(amount, currency);
    }

    // Existing Add method...
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot add different currencies: {Currency} and {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    // ADD subtract method
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot subtract different currencies: {Currency} and {other.Currency}");

        var result = Amount - other.Amount;

        if (result < 0)
            throw new DomainException("Subtraction would result in negative amount");

        return new Money(result, Currency);
    }

    // BONUS: Add comparison and multiplication
    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new DomainException("Factor cannot be negative");

        return new Money(Amount * factor, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Cannot compare different currencies");

        return Amount > other.Amount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
