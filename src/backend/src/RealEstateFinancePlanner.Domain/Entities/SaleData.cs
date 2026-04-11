namespace RealEstateFinancePlanner.Domain.Entities;

public class SaleData
{
    public decimal SalePrice { get; set; }
    public decimal SaleRelatedCosts { get; set; }
    public decimal OutstandingMortgageDebt { get; set; }
    public decimal CurrentCashBalance { get; set; }
    public decimal MunicipalCapitalGainsTax { get; set; }
    public decimal ExtraordinaryCosts { get; set; }
}
