namespace RealEstateFinancePlanner.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        Amount = amount;
    }

    public static Money Zero => new(0m);

    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);
    public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount);
    public static Money operator *(Money a, decimal factor) => new(a.Amount * factor);
    public static Money operator *(decimal factor, Money a) => new(a.Amount * factor);

    public static implicit operator decimal(Money m) => m.Amount;
    public static explicit operator Money(decimal d) => new(d);

    public override string ToString() => Amount.ToString("N2");
}
