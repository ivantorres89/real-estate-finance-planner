namespace RealEstateFinancePlanner.Domain.Entities;

public class PurchaseData
{
    public decimal OfficialPurchasePriceA { get; set; }
    public decimal UnofficialPurchasePriceB { get; set; }
    public decimal AppraisalValue { get; set; }
    public decimal FinanceablePercentage { get; set; } = 80m;
    public decimal NotaryCosts { get; set; }
    public decimal AdministrativeCosts { get; set; }
    public decimal AppraisalCosts { get; set; }
    public decimal AgencyCosts { get; set; }
    public decimal AgencyCostsB { get; set; }
    public decimal OtherCosts { get; set; }
    public decimal OtherCostsB { get; set; }
    public decimal RenovationCostsB { get; set; }
    public bool ApplyReducedItp { get; set; }
    public bool IsMainResidence { get; set; } = true;
    public int BuyerAge { get; set; }

    public Dictionary<string, object>? LegacyExtraElements { get; set; }
}
