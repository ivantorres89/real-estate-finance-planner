namespace RealEstateFinancePlanner.Domain.ValueObjects;

public readonly record struct Percentage
{
    public decimal Value { get; }

    public Percentage(decimal value)
    {
        if (value < 0m || value > 100m)
            throw new ArgumentOutOfRangeException(nameof(value), "Percentage must be between 0 and 100.");
        Value = value;
    }

    public decimal AsDecimalFraction => Value / 100m;

    public static Percentage Zero => new(0m);

    public static Percentage operator +(Percentage a, Percentage b) => new(a.Value + b.Value);
    public static Percentage operator -(Percentage a, Percentage b) => new(a.Value - b.Value);

    public override string ToString() => $"{Value:N2}%";
}
