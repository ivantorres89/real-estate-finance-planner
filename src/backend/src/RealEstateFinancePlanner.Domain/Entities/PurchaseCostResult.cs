namespace RealEstateFinancePlanner.Domain.Entities;

public class PurchaseCostResult
{
    public decimal ItpAmount { get; set; }
    public decimal MaxMortgageAmount { get; set; }
    public decimal EntryPayment { get; set; }
    public decimal NotaryCosts { get; set; }
    public decimal AdministrativeCosts { get; set; }
    public decimal AppraisalCosts { get; set; }
    public decimal AgencyCosts { get; set; }
    public decimal OtherCosts { get; set; }
    public decimal TotalCashNeeded { get; set; }
    public decimal RemainingLiquidity { get; set; }
    public bool IsViable { get; set; }
}
