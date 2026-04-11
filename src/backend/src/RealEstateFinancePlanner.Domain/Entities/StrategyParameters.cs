using RealEstateFinancePlanner.Domain.Enums;

namespace RealEstateFinancePlanner.Domain.Entities;

public class StrategyParameters
{
    public decimal ExpectedAnnualReturnConservative { get; set; } = 3m;
    public decimal ExpectedAnnualReturnBase { get; set; } = 6m;
    public decimal ExpectedAnnualReturnOptimistic { get; set; } = 9m;
    public bool UseNetReturns { get; set; } = true;
    public decimal? GrossExpectedReturn { get; set; }
    public decimal? NetExpectedReturn { get; set; }
    public int AnalysisHorizonYears { get; set; } = 20;
    public decimal MinimumLiquidityCushion { get; set; } = 10000m;
    public RiskProfile RiskProfile { get; set; } = RiskProfile.Balanced;
    public decimal AdditionalCapitalToPreserve { get; set; }
}
