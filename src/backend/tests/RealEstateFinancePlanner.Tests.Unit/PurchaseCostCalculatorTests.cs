using FluentAssertions;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Services;

namespace RealEstateFinancePlanner.Tests.Unit;

public class PurchaseCostCalculatorTests
{
    [Fact]
    public void Calculate_ReducedItp3Percent_CorrectItpAmount()
    {
        var purchase = new PurchaseData
        {
            PurchasePrice = 300_000m,
            DeedPrice = 280_000m,
            FinanceablePercentage = 80m,
            ApplyReducedItp = true,
            NotaryCosts = 1_500m,
            AdministrativeCosts = 800m,
            AppraisalCosts = 400m,
            AgencyCosts = 0m,
            OtherCosts = 0m,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, 200_000m);

        // ITP = 280000 * 0.03 = 8400
        result.ItpAmount.Should().Be(8_400m);
    }

    [Fact]
    public void Calculate_StandardItp6Percent_CorrectItpAmount()
    {
        var purchase = new PurchaseData
        {
            PurchasePrice = 300_000m,
            DeedPrice = 280_000m,
            FinanceablePercentage = 80m,
            ApplyReducedItp = false,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, 200_000m);

        // ITP = 280000 * 0.06 = 16800
        result.ItpAmount.Should().Be(16_800m);
    }

    [Fact]
    public void Calculate_MaxMortgage90Percent_CorrectAmount()
    {
        var purchase = new PurchaseData
        {
            PurchasePrice = 300_000m,
            DeedPrice = 280_000m,
            FinanceablePercentage = 90m,
            ApplyReducedItp = true,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, 500_000m);

        // Max mortgage = 280000 * 0.90 = 252000
        result.MaxMortgageAmount.Should().Be(252_000m);
        // Entry = 300000 - 252000 = 48000
        result.EntryPayment.Should().Be(48_000m);
    }

    [Fact]
    public void Calculate_TotalCashNeeded_IncludesAllCosts()
    {
        var purchase = new PurchaseData
        {
            PurchasePrice = 300_000m,
            DeedPrice = 280_000m,
            FinanceablePercentage = 80m,
            ApplyReducedItp = true,
            NotaryCosts = 1_500m,
            AdministrativeCosts = 800m,
            AppraisalCosts = 400m,
            AgencyCosts = 3_000m,
            OtherCosts = 500m,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, 200_000m);

        // Max mortgage = 280000 * 0.80 = 224000
        // Entry = 300000 - 224000 = 76000
        // ITP = 280000 * 0.03 = 8400
        // Total cash = 76000 + 8400 + 1500 + 800 + 400 + 3000 + 500 = 90600
        result.MaxMortgageAmount.Should().Be(224_000m);
        result.EntryPayment.Should().Be(76_000m);
        result.TotalCashNeeded.Should().Be(90_600m);
        // Remaining = 200000 - 90600 = 109400
        result.RemainingLiquidity.Should().Be(109_400m);
        result.IsViable.Should().BeTrue();
    }

    [Fact]
    public void Calculate_InsufficientFunds_NotViable()
    {
        var purchase = new PurchaseData
        {
            PurchasePrice = 300_000m,
            DeedPrice = 280_000m,
            FinanceablePercentage = 80m,
            ApplyReducedItp = true,
            NotaryCosts = 1_500m,
            AdministrativeCosts = 800m,
            AppraisalCosts = 400m,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, 50_000m);

        result.IsViable.Should().BeFalse();
        result.RemainingLiquidity.Should().BeNegative();
    }

    [Fact]
    public void Calculate_EntryPaymentFloor_NeverNegative()
    {
        var purchase = new PurchaseData
        {
            PurchasePrice = 200_000m,
            DeedPrice = 280_000m,
            FinanceablePercentage = 100m,
            ApplyReducedItp = true,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, 500_000m);

        // Max mortgage = 280000 * 1.0 = 280000 > 200000, so entry = 0
        result.EntryPayment.Should().Be(0m);
    }

    [Fact]
    public void Calculate_InvalidPurchasePrice_ThrowsArgumentException()
    {
        var purchase = new PurchaseData { PurchasePrice = 0m, DeedPrice = 100_000m };
        var act = () => PurchaseCostCalculator.Calculate(purchase, 100_000m);
        act.Should().Throw<ArgumentException>();
    }
}
