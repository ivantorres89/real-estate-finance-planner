using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Domain.Services;

public static class SaleLiquidityCalculator
{
    public static SaleLiquidityResult Calculate(SaleData sale)
    {
        ArgumentNullException.ThrowIfNull(sale);

        if (sale.SalePrice < 0)
            throw new ArgumentException("Sale price cannot be negative.", nameof(sale));

        decimal netSaleLiquidity =
            sale.SalePrice
            - sale.SaleRelatedCosts
            - sale.OutstandingMortgageDebt
            - sale.MunicipalCapitalGainsTax
            - sale.ExtraordinaryCosts;

        decimal realAvailableCash = sale.CurrentCashBalance + netSaleLiquidity;

        return new SaleLiquidityResult
        {
            SalePrice = sale.SalePrice,
            SaleRelatedCosts = sale.SaleRelatedCosts,
            OutstandingMortgageDebt = sale.OutstandingMortgageDebt,
            MunicipalCapitalGainsTax = sale.MunicipalCapitalGainsTax,
            ExtraordinaryCosts = sale.ExtraordinaryCosts,
            NetSaleLiquidity = netSaleLiquidity,
            CurrentCashBalance = sale.CurrentCashBalance,
            RealAvailableCash = realAvailableCash
        };
    }
}
