namespace RealEstateFinancePlanner.Domain.Entities;

public class BonusEvaluation
{
    public string BonusId { get; set; } = string.Empty;
    public string BonusName { get; set; } = string.Empty;
    public decimal InterestSavings { get; set; }
    public decimal TotalBonusCost { get; set; }
    public decimal NetGainOrLoss { get; set; }
    public bool IsWorthIt { get; set; }
}
