using RealEstateFinancePlanner.Domain.Enums;

namespace RealEstateFinancePlanner.Domain.Entities;

public class StrategyResult
{
    public decimal EntryPayment { get; set; }
    public decimal MortgagePrincipal { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal TotalBonusCost { get; set; }
    public decimal RemainingLiquidity { get; set; }
    public decimal DebtRatio { get; set; }
    public decimal TotalFinancialCost { get; set; }
    public decimal? FutureValueConservative { get; set; }
    public decimal? FutureValueBase { get; set; }
    public decimal? FutureValueOptimistic { get; set; }
}

public class StrategyComparisonResult
{
    public StrategyResult AmortizeFaster { get; set; } = new();
    public StrategyResult MaintainCapital { get; set; } = new();
    public decimal NetDifferenceConservative { get; set; }
    public decimal NetDifferenceBase { get; set; }
    public decimal NetDifferenceOptimistic { get; set; }
    public decimal WeightedNetDifference { get; set; }
    public StrategyRecommendation Recommendation { get; set; }
    public string Explanation { get; set; } = string.Empty;
}
