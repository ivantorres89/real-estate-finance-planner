using FluentAssertions;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Services;

namespace RealEstateFinancePlanner.Tests.Unit;

public class SaleLiquidityCalculatorTests
{
    [Fact]
    public void Calculate_UserGoldenCase_SplitsAccountingAB()
    {
        // Reference scenario provided by the user:
        // A=170.000, B=100.000, deuda=168.149 → A apenas cubre la deuda.
        var sale = new SaleData
        {
            OfficialSalePriceA = 170_000m,
            UnofficialSalePriceB = 100_000m,
            SaleRelatedCosts = 0m,
            OutstandingMortgageDebt = 168_149m,
            MunicipalCapitalGainsTax = 0m,
            ExtraordinaryCosts = 0m,
            CurrentCashBalance = 0m,
        };

        var result = SaleLiquidityCalculator.Calculate(sale);

        result.TotalSalePrice.Should().Be(270_000m);
        result.TotalSaleDeductionsA.Should().Be(168_149m);
        result.NetSaleLiquidityA.Should().Be(1_851m);
        result.NetSaleLiquidityB.Should().Be(100_000m);
        result.RealAvailableCashA.Should().Be(1_851m);
        result.RealAvailableCashB.Should().Be(100_000m);
        result.TotalRealAvailableCash.Should().Be(101_851m);
        result.IsSaleViable.Should().BeTrue();
    }

    [Fact]
    public void Calculate_AInsufficientWithoutCash_NotViable()
    {
        var sale = new SaleData
        {
            OfficialSalePriceA = 165_000m,
            UnofficialSalePriceB = 100_000m,
            SaleRelatedCosts = 2_000m,
            OutstandingMortgageDebt = 168_149m,
            CurrentCashBalance = 0m,
        };

        var result = SaleLiquidityCalculator.Calculate(sale);

        result.TotalSaleDeductionsA.Should().Be(170_149m);
        result.NetSaleLiquidityA.Should().Be(-5_149m);
        result.RealAvailableCashA.Should().Be(-5_149m);
        result.IsSaleViable.Should().BeFalse();
        result.NetSaleLiquidityB.Should().Be(100_000m);
    }

    [Fact]
    public void Calculate_AInsufficientCoveredWithCurrentCash_Viable()
    {
        var sale = new SaleData
        {
            OfficialSalePriceA = 165_000m,
            UnofficialSalePriceB = 100_000m,
            SaleRelatedCosts = 2_000m,
            OutstandingMortgageDebt = 168_149m,
            CurrentCashBalance = 10_000m,
        };

        var result = SaleLiquidityCalculator.Calculate(sale);

        result.NetSaleLiquidityA.Should().Be(-5_149m);
        result.RealAvailableCashA.Should().Be(4_851m);
        result.IsSaleViable.Should().BeTrue();
    }

    [Fact]
    public void Calculate_BIsZero_FullOfficialOperation()
    {
        var sale = new SaleData
        {
            OfficialSalePriceA = 250_000m,
            UnofficialSalePriceB = 0m,
            SaleRelatedCosts = 5_000m,
            OutstandingMortgageDebt = 120_000m,
            CurrentCashBalance = 30_000m,
            MunicipalCapitalGainsTax = 2_000m,
            ExtraordinaryCosts = 1_000m,
        };

        var result = SaleLiquidityCalculator.Calculate(sale);

        result.TotalSalePrice.Should().Be(250_000m);
        result.TotalSaleDeductionsA.Should().Be(128_000m);
        result.NetSaleLiquidityA.Should().Be(122_000m);
        result.NetSaleLiquidityB.Should().Be(0m);
        result.RealAvailableCashA.Should().Be(152_000m);
        result.RealAvailableCashB.Should().Be(0m);
        result.TotalRealAvailableCash.Should().Be(152_000m);
        result.IsSaleViable.Should().BeTrue();
    }

    [Fact]
    public void Calculate_NoDebtNoCosts_FullPriceFlowsThrough()
    {
        var sale = new SaleData
        {
            OfficialSalePriceA = 280_000m,
            UnofficialSalePriceB = 20_000m,
            CurrentCashBalance = 10_000m,
        };

        var result = SaleLiquidityCalculator.Calculate(sale);

        result.NetSaleLiquidityA.Should().Be(280_000m);
        result.NetSaleLiquidityB.Should().Be(20_000m);
        result.RealAvailableCashA.Should().Be(290_000m);
        result.RealAvailableCashB.Should().Be(20_000m);
    }

    [Fact]
    public void Calculate_NullSale_ThrowsArgumentNullException()
    {
        var act = () => SaleLiquidityCalculator.Calculate(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Calculate_NegativeOfficialPriceA_ThrowsArgumentException()
    {
        var sale = new SaleData { OfficialSalePriceA = -1m };
        var act = () => SaleLiquidityCalculator.Calculate(sale);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Official sale price A*");
    }

    [Fact]
    public void Calculate_NegativeUnofficialPriceB_ThrowsArgumentException()
    {
        var sale = new SaleData { OfficialSalePriceA = 1m, UnofficialSalePriceB = -1m };
        var act = () => SaleLiquidityCalculator.Calculate(sale);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Unofficial sale price B*");
    }
}
