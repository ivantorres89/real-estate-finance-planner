namespace RealEstateFinancePlanner.Domain.Entities;

public class PurchaseData
{
    public decimal PurchasePrice { get; set; }
    public decimal DeedPrice { get; set; }
    public decimal FinanceablePercentage { get; set; } = 80m;
    public decimal NotaryCosts { get; set; }
    public decimal AdministrativeCosts { get; set; }
    public decimal AppraisalCosts { get; set; }
    public decimal AgencyCosts { get; set; }
    public decimal OtherCosts { get; set; }
    public bool ApplyReducedItp { get; set; }
    public bool IsMainResidence { get; set; } = true;
    public int BuyerAge { get; set; }
}
