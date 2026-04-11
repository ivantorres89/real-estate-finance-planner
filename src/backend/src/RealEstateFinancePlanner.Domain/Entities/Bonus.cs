using RealEstateFinancePlanner.Domain.Enums;

namespace RealEstateFinancePlanner.Domain.Entities;

public class Bonus
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public BonusCategory Category { get; set; }
    public decimal TinReductionPercentage { get; set; }
    public decimal MonthlyCost { get; set; }
    public decimal YearlyCost { get; set; }
    public decimal OneTimeCost { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsAccepted { get; set; }
    public int? DurationYears { get; set; }
    public string? Notes { get; set; }
}
