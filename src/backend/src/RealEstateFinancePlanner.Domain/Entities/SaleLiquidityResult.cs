namespace RealEstateFinancePlanner.Domain.Entities;

public class SaleLiquidityResult
{
    public decimal OfficialSalePriceA { get; set; }
    public decimal UnofficialSalePriceB { get; set; }
    public decimal TotalSalePrice { get; set; }
    public decimal SaleRelatedCosts { get; set; }
    public decimal OutstandingMortgageDebt { get; set; }
    public decimal MunicipalCapitalGainsTax { get; set; }
    public decimal ExtraordinaryCosts { get; set; }
    public decimal TotalSaleDeductionsA { get; set; }
    public decimal NetSaleLiquidityA { get; set; }
    public decimal NetSaleLiquidityB { get; set; }
    public decimal CurrentCashBalance { get; set; }
    public decimal RealAvailableCashA { get; set; }
    public decimal RealAvailableCashB { get; set; }
    public decimal TotalRealAvailableCash { get; set; }
    public bool IsSaleViable { get; set; }
}
