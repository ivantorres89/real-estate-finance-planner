using FluentAssertions;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Services;

namespace RealEstateFinancePlanner.Tests.Unit;

public class PurchaseCostCalculatorTests
{
    [Fact]
    public void Calculate_UserBalancedCase_OperationSquaredNoIdleB()
    {
        // User's cuadre target: A=200k escritura, B=85k cash al vendedor,
        // tasación=200k, financiable 90%, ITP reducido, reformas B=15k.
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 200_000m,
            UnofficialPurchasePriceB = 85_000m,
            AppraisalValue = 200_000m,
            FinanceablePercentage = 90m,
            ApplyReducedItp = true,
            NotaryCosts = 1_500m,
            AdministrativeCosts = 800m,
            AppraisalCosts = 400m,
            AgencyCosts = 0m,
            OtherCosts = 800m,
            RenovationCostsB = 15_000m,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, realAvailableCashA: 30_000m, realAvailableCashB: 100_000m);

        result.TotalPurchasePrice.Should().Be(285_000m);
        result.EffectiveAppraisalValue.Should().Be(200_000m);
        result.MortgageBaseValue.Should().Be(200_000m);
        result.MaxMortgageAmount.Should().Be(180_000m);
        result.ItpAmount.Should().Be(6_000m);
        result.EntryPaymentA.Should().Be(20_000m);
        result.EntryPaymentB.Should().Be(85_000m);
        result.TotalCashNeededA.Should().Be(29_500m);
        result.TotalCashNeededB.Should().Be(100_000m);
        result.RemainingLiquidityA.Should().Be(500m);
        result.RemainingLiquidityB.Should().Be(0m);
        result.IdleCashB.Should().Be(0m);
        result.IsViable.Should().BeTrue();
        result.BalancingAdvice.Should().Contain("Operación cuadrada");
    }

    [Fact]
    public void Calculate_AppraisalBelowA_MortgageCappedAndAdviceMentionsTasation()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 220_000m,
            UnofficialPurchasePriceB = 0m,
            AppraisalValue = 200_000m,
            FinanceablePercentage = 90m,
            ApplyReducedItp = true,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, realAvailableCashA: 80_000m, realAvailableCashB: 0m);

        result.EffectiveAppraisalValue.Should().Be(200_000m);
        result.MortgageBaseValue.Should().Be(200_000m);
        result.MaxMortgageAmount.Should().Be(180_000m);
        result.EntryPaymentA.Should().Be(40_000m);
        result.BalancingAdvice.Should().Contain("tasación");
    }

    [Fact]
    public void Calculate_AppraisalZero_FallsBackToOfficialPriceA()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 250_000m,
            UnofficialPurchasePriceB = 0m,
            AppraisalValue = 0m,
            FinanceablePercentage = 80m,
            ApplyReducedItp = true,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, realAvailableCashA: 100_000m, realAvailableCashB: 0m);

        result.EffectiveAppraisalValue.Should().Be(250_000m);
        result.MortgageBaseValue.Should().Be(250_000m);
        result.MaxMortgageAmount.Should().Be(200_000m);
        result.EntryPaymentA.Should().Be(50_000m);
    }

    [Fact]
    public void Calculate_IdleCashB_AboveThresholdRaisesWarning()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 200_000m,
            UnofficialPurchasePriceB = 70_000m,
            AppraisalValue = 200_000m,
            FinanceablePercentage = 90m,
            ApplyReducedItp = true,
            RenovationCostsB = 20_000m,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, realAvailableCashA: 30_000m, realAvailableCashB: 100_000m);

        result.RemainingLiquidityB.Should().Be(10_000m);
        result.IdleCashB.Should().Be(10_000m);
        result.BalancingAdvice.Should().Contain("B ocioso");
    }

    [Fact]
    public void Calculate_ADeficit_FlagsNonViableAndAdvice()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 200_000m,
            UnofficialPurchasePriceB = 0m,
            AppraisalValue = 200_000m,
            FinanceablePercentage = 90m,
            ApplyReducedItp = true,
            NotaryCosts = 1_500m,
            OtherCosts = 800m,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, realAvailableCashA: 10_000m, realAvailableCashB: 0m);

        result.RemainingLiquidityA.Should().BeLessThan(0m);
        result.IsViable.Should().BeFalse();
        result.BalancingAdvice.Should().Contain("La parte A no cubre");
    }

    [Fact]
    public void Calculate_BDeficit_FlagsNonViableAndAdvice()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 200_000m,
            UnofficialPurchasePriceB = 60_000m,
            AppraisalValue = 200_000m,
            FinanceablePercentage = 90m,
            ApplyReducedItp = true,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, realAvailableCashA: 30_000m, realAvailableCashB: 50_000m);

        result.RemainingLiquidityB.Should().Be(-10_000m);
        result.IsViable.Should().BeFalse();
        result.BalancingAdvice.Should().Contain("La parte B no cubre");
    }

    [Fact]
    public void Calculate_StandardItp6Percent_AppliedToOfficialPriceA()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 280_000m,
            UnofficialPurchasePriceB = 0m,
            AppraisalValue = 280_000m,
            FinanceablePercentage = 80m,
            ApplyReducedItp = false,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, realAvailableCashA: 100_000m, realAvailableCashB: 0m);

        result.ItpAmount.Should().Be(16_800m);
    }

    [Fact]
    public void Calculate_NegativeRealAvailableCashA_PropagatesAdviceForSaleSide()
    {
        // realAvailableCashA < 0 means the sale-side already failed; the advice must
        // mention that the A part of the sale doesn't cover the debt.
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 200_000m,
            UnofficialPurchasePriceB = 0m,
            AppraisalValue = 200_000m,
            FinanceablePercentage = 90m,
            ApplyReducedItp = true,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, realAvailableCashA: -1_149m, realAvailableCashB: 100_000m);

        result.IsViable.Should().BeFalse();
        result.BalancingAdvice.Should().Contain("La parte A de la venta no cubre");
    }

    [Fact]
    public void Calculate_NullPurchase_Throws()
    {
        var act = () => PurchaseCostCalculator.Calculate(null!, 0m, 0m);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Calculate_OfficialPriceAZero_Throws()
    {
        var purchase = new PurchaseData { OfficialPurchasePriceA = 0m };
        var act = () => PurchaseCostCalculator.Calculate(purchase, 0m, 0m);
        act.Should().Throw<ArgumentException>().WithMessage("*Official purchase price A*");
    }

    [Fact]
    public void Calculate_NegativeUnofficialPriceB_Throws()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 100_000m,
            UnofficialPurchasePriceB = -1m,
        };
        var act = () => PurchaseCostCalculator.Calculate(purchase, 0m, 0m);
        act.Should().Throw<ArgumentException>().WithMessage("*Unofficial purchase price B*");
    }

    [Fact]
    public void Calculate_NegativeAppraisal_Throws()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 100_000m,
            AppraisalValue = -1m,
        };
        var act = () => PurchaseCostCalculator.Calculate(purchase, 0m, 0m);
        act.Should().Throw<ArgumentException>().WithMessage("*Appraisal value*");
    }

    [Fact]
    public void Calculate_NegativeRenovationCostsB_Throws()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 100_000m,
            RenovationCostsB = -1m,
        };
        var act = () => PurchaseCostCalculator.Calculate(purchase, 0m, 0m);
        act.Should().Throw<ArgumentException>().WithMessage("*Renovation costs B*");
    }

    [Fact]
    public void Calculate_AgencyAndOtherInB_AggregatedIntoTotalCashNeededB()
    {
        // Same user shape but agencia and "otros" pagados en B.
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 200_000m,
            UnofficialPurchasePriceB = 70_000m,
            AppraisalValue = 200_000m,
            FinanceablePercentage = 90m,
            ApplyReducedItp = true,
            NotaryCosts = 1_500m,
            AdministrativeCosts = 800m,
            AppraisalCosts = 400m,
            AgencyCosts = 0m,
            AgencyCostsB = 5_000m,
            OtherCosts = 0m,
            OtherCostsB = 500m,
            RenovationCostsB = 15_000m,
        };

        var result = PurchaseCostCalculator.Calculate(purchase, realAvailableCashA: 30_000m, realAvailableCashB: 100_000m);

        // A side: 20.000 entrada + 6.000 ITP + 1.500 + 800 + 400 = 28.700
        result.TotalCashNeededA.Should().Be(28_700m);
        // B side: 70.000 entrega vendedor + 15.000 reformas + 5.000 agencia + 500 otros = 90.500
        result.TotalCashNeededB.Should().Be(90_500m);
        result.RemainingLiquidityA.Should().Be(1_300m);
        result.RemainingLiquidityB.Should().Be(9_500m);
        result.IdleCashB.Should().Be(9_500m);
        result.IsViable.Should().BeTrue();
        result.BalancingAdvice.Should().Contain("B ocioso");
    }

    [Fact]
    public void Calculate_NegativeAgencyCostsB_Throws()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 100_000m,
            AgencyCostsB = -1m,
        };
        var act = () => PurchaseCostCalculator.Calculate(purchase, 0m, 0m);
        act.Should().Throw<ArgumentException>().WithMessage("*Agency costs B*");
    }

    [Fact]
    public void Calculate_NegativeOtherCostsB_Throws()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 100_000m,
            OtherCostsB = -1m,
        };
        var act = () => PurchaseCostCalculator.Calculate(purchase, 0m, 0m);
        act.Should().Throw<ArgumentException>().WithMessage("*Other costs B*");
    }

    [Fact]
    public void Calculate_FinanceablePercentageOutOfRange_Throws()
    {
        var purchase = new PurchaseData
        {
            OfficialPurchasePriceA = 100_000m,
            FinanceablePercentage = 150m,
        };
        var act = () => PurchaseCostCalculator.Calculate(purchase, 0m, 0m);
        act.Should().Throw<ArgumentException>().WithMessage("*Financeable percentage*");
    }
}
