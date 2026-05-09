namespace RealEstateFinancePlanner.Domain.Entities;

public class SaleLiquidityResult
{
    public decimal SalePrice { get; set; }
    public decimal SaleRelatedCosts { get; set; }
    public decimal OutstandingMortgageDebt { get; set; }
    public decimal MunicipalCapitalGainsTax { get; set; }
    public decimal ExtraordinaryCosts { get; set; }
    public decimal NetSaleLiquidity { get; set; }
    public decimal CurrentCashBalance { get; set; }
    public decimal RealAvailableCash { get; set; }
}
