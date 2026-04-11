using FluentAssertions;
using RealEstateFinancePlanner.Domain.ValueObjects;

namespace RealEstateFinancePlanner.Tests.Unit;

public class ValueObjectTests
{
    [Fact]
    public void Money_ArithmeticOperations_Work()
    {
        var a = new Money(100m);
        var b = new Money(50m);

        (a + b).Amount.Should().Be(150m);
        (a - b).Amount.Should().Be(50m);
        (a * 2m).Amount.Should().Be(200m);
        (3m * b).Amount.Should().Be(150m);
    }

    [Fact]
    public void Money_ImplicitConversion_ToDecimal()
    {
        var m = new Money(42.50m);
        decimal d = m;
        d.Should().Be(42.50m);
    }

    [Fact]
    public void Percentage_ValidRange_Works()
    {
        var p = new Percentage(35m);
        p.Value.Should().Be(35m);
        p.AsDecimalFraction.Should().Be(0.35m);
    }

    [Fact]
    public void Percentage_OutOfRange_Throws()
    {
        var act1 = () => new Percentage(-1m);
        act1.Should().Throw<ArgumentOutOfRangeException>();

        var act2 = () => new Percentage(101m);
        act2.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Percentage_Addition_Works()
    {
        var a = new Percentage(20m);
        var b = new Percentage(15m);
        (a + b).Value.Should().Be(35m);
    }
}
