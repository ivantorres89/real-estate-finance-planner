using FluentAssertions;
using RealEstateFinancePlanner.Domain.Services;

namespace RealEstateFinancePlanner.Tests.Unit;

public class MortgageCalculatorTests
{
    [Fact]
    public void Calculate_FixedRate_CorrectMonthlyPayment()
    {
        // 200000 EUR at 2.5% TIN for 25 years
        var result = MortgageCalculator.Calculate(200_000m, 2.5m, 25, 3_500m, 0m);

        // Expected monthly payment ~897.22 EUR (French amortization)
        result.MonthlyPayment.Should().BeApproximately(897.22m, 0.01m);
        result.TotalPaid.Should().BeApproximately(269_166m, 5m);
        result.TotalInterest.Should().BeApproximately(69_166m, 5m);
    }

    [Fact]
    public void Calculate_FixedRate_CorrectDebtRatio()
    {
        var result = MortgageCalculator.Calculate(200_000m, 2.5m, 25, 3_500m, 200m);

        // Debt ratio = (897.22 + 200) / 3500 * 100 = ~31.35%
        result.ResultingDebtRatio.Should().BeInRange(31m, 32m);
        result.RemainingMonthlyFreeCash.Should().BeApproximately(3_500m - 897.22m - 200m, 1m);
    }

    [Fact]
    public void Calculate_ZeroInterestRate_SimpleAmortization()
    {
        var result = MortgageCalculator.Calculate(120_000m, 0m, 10, 3_000m, 0m);

        // 120000 / 120 = 1000
        result.MonthlyPayment.Should().Be(1_000m);
        result.TotalPaid.Should().Be(120_000m);
        result.TotalInterest.Should().Be(0m);
    }

    [Fact]
    public void Calculate_ShortTerm15Years_HigherPaymentLessInterest()
    {
        var result15 = MortgageCalculator.Calculate(200_000m, 2.5m, 15, 4_000m, 0m);
        var result30 = MortgageCalculator.Calculate(200_000m, 2.5m, 30, 4_000m, 0m);

        result15.MonthlyPayment.Should().BeGreaterThan(result30.MonthlyPayment);
        result15.TotalInterest.Should().BeLessThan(result30.TotalInterest);
    }

    [Fact]
    public void Calculate_HighTin_SignificantInterest()
    {
        // 300000 at 4% for 30 years
        var result = MortgageCalculator.Calculate(300_000m, 4.0m, 30, 5_000m, 0m);

        // Monthly ~1432.25, total ~515610, interest ~215610
        result.MonthlyPayment.Should().BeApproximately(1432.25m, 0.5m);
        result.TotalInterest.Should().BeGreaterThan(200_000m);
    }

    [Fact]
    public void Calculate_NegativePrincipal_ThrowsArgumentException()
    {
        var act = () => MortgageCalculator.Calculate(-1m, 2.5m, 25, 3000m, 0m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Calculate_ZeroTermYears_ThrowsArgumentException()
    {
        var act = () => MortgageCalculator.Calculate(100_000m, 2.5m, 0, 3000m, 0m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DecimalPow_CorrectPrecision()
    {
        // (1.002083)^300 should be close to 1.867 for 2.5% / 12
        decimal monthlyRate = 2.5m / 100m / 12m;
        decimal result = MortgageCalculator.DecimalPow(1m + monthlyRate, 300);

        result.Should().BeApproximately(1.867m, 0.01m);
    }

    [Fact]
    public void Calculate_AllMonetaryValuesAreDecimal()
    {
        var result = MortgageCalculator.Calculate(200_000m, 2.5m, 25, 3_500m, 0m);

        // Verify these are all decimals, no double precision issues
        result.MonthlyPayment.Should().BeOfType(typeof(decimal));
        result.TotalPaid.Should().BeOfType(typeof(decimal));
        result.TotalInterest.Should().BeOfType(typeof(decimal));
    }
}
