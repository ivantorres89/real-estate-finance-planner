using FluentAssertions;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Services;

namespace RealEstateFinancePlanner.Tests.Unit;

public class DebtCapacityCalculatorTests
{
    [Fact]
    public void Calculate_StandardScenario_CorrectCapacity()
    {
        var data = new DebtCapacityData
        {
            MonthlyNetSalary = 3_500m,
            MonthlyOutstandingLoanPayments = 200m,
            MaxDebtRatioPercentage = 35m
        };

        var result = DebtCapacityCalculator.Calculate(data);

        // 3500 * 0.35 = 1225 - 200 = 1025
        result.MaxMonthlyPaymentCapacity.Should().Be(1_025m);
    }

    [Fact]
    public void Calculate_NoExistingLoans_FullCapacity()
    {
        var data = new DebtCapacityData
        {
            MonthlyNetSalary = 4_000m,
            MonthlyOutstandingLoanPayments = 0m,
            MaxDebtRatioPercentage = 35m
        };

        var result = DebtCapacityCalculator.Calculate(data);

        // 4000 * 0.35 = 1400
        result.MaxMonthlyPaymentCapacity.Should().Be(1_400m);
    }

    [Fact]
    public void Calculate_HighExistingLoans_ZeroCapacity()
    {
        var data = new DebtCapacityData
        {
            MonthlyNetSalary = 3_000m,
            MonthlyOutstandingLoanPayments = 2_000m,
            MaxDebtRatioPercentage = 35m
        };

        var result = DebtCapacityCalculator.Calculate(data);

        // 3000 * 0.35 = 1050 - 2000 = -950, floored to 0
        result.MaxMonthlyPaymentCapacity.Should().Be(0m);
    }

    [Fact]
    public void Calculate_CustomDebtRatio_40Percent()
    {
        var data = new DebtCapacityData
        {
            MonthlyNetSalary = 5_000m,
            MonthlyOutstandingLoanPayments = 300m,
            MaxDebtRatioPercentage = 40m
        };

        var result = DebtCapacityCalculator.Calculate(data);

        // 5000 * 0.40 = 2000 - 300 = 1700
        result.MaxMonthlyPaymentCapacity.Should().Be(1_700m);
    }

    [Fact]
    public void Calculate_NegativeSalary_ThrowsArgumentException()
    {
        var data = new DebtCapacityData { MonthlyNetSalary = -1000m };
        var act = () => DebtCapacityCalculator.Calculate(data);
        act.Should().Throw<ArgumentException>();
    }
}
