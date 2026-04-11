namespace RealEstateFinancePlanner.Domain.Entities;

public class Scenario
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SaleData Sale { get; set; } = new();
    public PurchaseData Purchase { get; set; } = new();
    public DebtCapacityData DebtCapacity { get; set; } = new();
    public List<BankOffer> Banks { get; set; } = [];
    public StrategyParameters StrategyParameters { get; set; } = new();
    public AnalysisResult? LastResult { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
