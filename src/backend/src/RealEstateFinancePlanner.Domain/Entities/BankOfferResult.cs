using RealEstateFinancePlanner.Domain.Enums;

namespace RealEstateFinancePlanner.Domain.Entities;

public class BankOfferResult
{
    public string BankId { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public decimal BaseTin { get; set; }
    public decimal MaxTheoreticalDiscountedTin { get; set; }
    public decimal FinalRealTin { get; set; }
    public List<MortgageCalculationResult> MortgagesByTerm { get; set; } = [];
    public decimal TotalAcceptedBonusCost { get; set; }
    public List<BonusEvaluation> BonusEvaluations { get; set; } = [];
    public decimal RealGlobalCost { get; set; }
    public BankRecommendationLevel Recommendation { get; set; }
}
