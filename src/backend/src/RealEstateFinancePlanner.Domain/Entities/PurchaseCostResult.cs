namespace RealEstateFinancePlanner.Domain.Entities;

public class PurchaseCostResult
{
    public decimal OfficialPurchasePriceA { get; set; }
    public decimal UnofficialPurchasePriceB { get; set; }
    public decimal TotalPurchasePrice { get; set; }
    public decimal AppraisalValue { get; set; }
    public decimal EffectiveAppraisalValue { get; set; }
    public decimal MortgageBaseValue { get; set; }
    public decimal FinanceablePercentage { get; set; }
    public decimal ItpAmount { get; set; }
    public decimal MaxMortgageAmount { get; set; }
    public decimal EntryPaymentA { get; set; }
    public decimal EntryPaymentB { get; set; }
    public decimal NotaryCosts { get; set; }
    public decimal AdministrativeCosts { get; set; }
    public decimal AppraisalCosts { get; set; }
    public decimal AgencyCosts { get; set; }
    public decimal AgencyCostsB { get; set; }
    public decimal OtherCosts { get; set; }
    public decimal OtherCostsB { get; set; }
    public decimal RenovationCostsB { get; set; }
    public decimal TotalCashNeededA { get; set; }
    public decimal TotalCashNeededB { get; set; }
    public decimal RemainingLiquidityA { get; set; }
    public decimal RemainingLiquidityB { get; set; }
    public decimal IdleCashB { get; set; }
    public bool IsViable { get; set; }
    public string BalancingAdvice { get; set; } = string.Empty;
}
