using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Domain.Services;

public static class SaleLiquidityCalculator
{
    public static SaleLiquidityResult Calculate(SaleData sale)
    {
        ArgumentNullException.ThrowIfNull(sale);

        if (sale.OfficialSalePriceA < 0)
            throw new ArgumentException("Official sale price A cannot be negative.", nameof(sale));
        if (sale.UnofficialSalePriceB < 0)
            throw new ArgumentException("Unofficial sale price B cannot be negative.", nameof(sale));

        decimal totalSalePrice = sale.OfficialSalePriceA + sale.UnofficialSalePriceB;

        decimal totalSaleDeductionsA =
            sale.SaleRelatedCosts
            + sale.OutstandingMortgageDebt
            + sale.MunicipalCapitalGainsTax
            + sale.ExtraordinaryCosts;

        decimal netSaleLiquidityA = sale.OfficialSalePriceA - totalSaleDeductionsA;
        decimal netSaleLiquidityB = sale.UnofficialSalePriceB;

        decimal realAvailableCashA = sale.CurrentCashBalance + netSaleLiquidityA;
        decimal realAvailableCashB = netSaleLiquidityB;
        decimal totalRealAvailableCash = realAvailableCashA + realAvailableCashB;

        return new SaleLiquidityResult
        {
            OfficialSalePriceA = sale.OfficialSalePriceA,
            UnofficialSalePriceB = sale.UnofficialSalePriceB,
            TotalSalePrice = totalSalePrice,
            SaleRelatedCosts = sale.SaleRelatedCosts,
            OutstandingMortgageDebt = sale.OutstandingMortgageDebt,
            MunicipalCapitalGainsTax = sale.MunicipalCapitalGainsTax,
            ExtraordinaryCosts = sale.ExtraordinaryCosts,
            TotalSaleDeductionsA = totalSaleDeductionsA,
            NetSaleLiquidityA = netSaleLiquidityA,
            NetSaleLiquidityB = netSaleLiquidityB,
            CurrentCashBalance = sale.CurrentCashBalance,
            RealAvailableCashA = realAvailableCashA,
            RealAvailableCashB = realAvailableCashB,
            TotalRealAvailableCash = totalRealAvailableCash,
            IsSaleViable = realAvailableCashA >= 0m,
        };
    }
}
