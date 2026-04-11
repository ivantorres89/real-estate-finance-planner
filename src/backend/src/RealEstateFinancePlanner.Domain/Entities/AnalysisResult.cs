namespace RealEstateFinancePlanner.Domain.Entities;

public class AnalysisResult
{
    public SaleLiquidityResult SaleLiquidity { get; set; } = new();
    public PurchaseCostResult PurchaseCosts { get; set; } = new();
    public DebtCapacityResult DebtCapacity { get; set; } = new();
    public List<BankOfferResult> BankResults { get; set; } = [];
    public StrategyComparisonResult? StrategyComparison { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}
