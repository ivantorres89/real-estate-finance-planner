using FluentAssertions;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Services;

namespace RealEstateFinancePlanner.Tests.Unit;

public class SaleLiquidityCalculatorTests
{
    [Fact]
    public void Calculate_StandardSale_ReturnsCorrectNetLiquidity()
    {
        var sale = new SaleData
        {
            SalePrice = 250_000m,
            SaleRelatedCosts = 5_000m,
            OutstandingMortgageDebt = 120_000m,
            CurrentCashBalance = 30_000m,
            MunicipalCapitalGainsTax = 2_000m,
            ExtraordinaryCosts = 1_000m
        };

        var result = SaleLiquidityCalculator.Calculate(sale);

        // 250000 - 5000 - 120000 - 2000 - 1000 = 122000
        result.NetSaleLiquidity.Should().Be(122_000m);
        // 30000 + 122000 = 152000
        result.RealAvailableCash.Should().Be(152_000m);
    }

    [Fact]
    public void Calculate_NoDebtNoCosts_EntireSalePriceIsNet()
    {
        var sale = new SaleData
        {
            SalePrice = 300_000m,
            CurrentCashBalance = 10_000m
        };

        var result = SaleLiquidityCalculator.Calculate(sale);

        result.NetSaleLiquidity.Should().Be(300_000m);
        result.RealAvailableCash.Should().Be(310_000m);
    }

    [Fact]
    public void Calculate_HighDebt_NegativeNetLiquidity()
    {
        var sale = new SaleData
        {
            SalePrice = 200_000m,
            SaleRelatedCosts = 5_000m,
            OutstandingMortgageDebt = 210_000m,
            CurrentCashBalance = 50_000m
        };

        var result = SaleLiquidityCalculator.Calculate(sale);

        result.NetSaleLiquidity.Should().Be(-15_000m);
        result.RealAvailableCash.Should().Be(35_000m);
    }

    [Fact]
    public void Calculate_NullSale_ThrowsArgumentNullException()
    {
        var act = () => SaleLiquidityCalculator.Calculate(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Calculate_NegativeSalePrice_ThrowsArgumentException()
    {
        var sale = new SaleData { SalePrice = -1m };
        var act = () => SaleLiquidityCalculator.Calculate(sale);
        act.Should().Throw<ArgumentException>();
    }
}
