using System.ComponentModel.DataAnnotations;
using RealEstateFinancePlanner.Domain.Enums;

namespace RealEstateFinancePlanner.Api.Dtos;

// ──── Request DTOs ────

public class CreateScenarioRequest
{
    [Required, MinLength(1), MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required] public SaleDataDto Sale { get; set; } = new();
    [Required] public PurchaseDataDto Purchase { get; set; } = new();
    [Required] public DebtCapacityDataDto DebtCapacity { get; set; } = new();
    public List<BankOfferDto> Banks { get; set; } = [];
    public StrategyParametersDto StrategyParameters { get; set; } = new();
}

public class UpdateScenarioRequest : CreateScenarioRequest { }

public class SaleDataDto
{
    [Range(0, double.MaxValue)] public decimal SalePrice { get; set; }
    [Range(0, double.MaxValue)] public decimal SaleRelatedCosts { get; set; }
    [Range(0, double.MaxValue)] public decimal OutstandingMortgageDebt { get; set; }
    [Range(0, double.MaxValue)] public decimal CurrentCashBalance { get; set; }
    [Range(0, double.MaxValue)] public decimal MunicipalCapitalGainsTax { get; set; }
    [Range(0, double.MaxValue)] public decimal ExtraordinaryCosts { get; set; }
}

public class PurchaseDataDto
{
    [Range(0, double.MaxValue)] public decimal PurchasePrice { get; set; }
    [Range(0, double.MaxValue)] public decimal DeedPrice { get; set; }
    [Range(0, 100)] public decimal FinanceablePercentage { get; set; } = 80m;
    [Range(0, double.MaxValue)] public decimal NotaryCosts { get; set; }
    [Range(0, double.MaxValue)] public decimal AdministrativeCosts { get; set; }
    [Range(0, double.MaxValue)] public decimal AppraisalCosts { get; set; }
    [Range(0, double.MaxValue)] public decimal AgencyCosts { get; set; }
    [Range(0, double.MaxValue)] public decimal OtherCosts { get; set; }
    public bool ApplyReducedItp { get; set; }
    public bool IsMainResidence { get; set; } = true;
    [Range(18, 100)] public int BuyerAge { get; set; }
}

public class DebtCapacityDataDto
{
    [Range(0, double.MaxValue)] public decimal MonthlyNetSalary { get; set; }
    [Range(0, double.MaxValue)] public decimal MonthlyOutstandingLoanPayments { get; set; }
    [Range(1, 100)] public decimal MaxDebtRatioPercentage { get; set; } = 35m;
}

public class BankOfferDto
{
    public string? Id { get; set; }
    [Required, MinLength(1)] public string BankName { get; set; } = string.Empty;
    [Range(0, 20)] public decimal BaseTinPercentage { get; set; }
    public List<BonusDto> Bonuses { get; set; } = [];
    public List<int> MortgageTermsYears { get; set; } = [15, 20, 25, 30];
}

public class BonusDto
{
    public string? Id { get; set; }
    [Required, MinLength(1)] public string Name { get; set; } = string.Empty;
    public BonusCategory Category { get; set; }
    [Range(0, 10)] public decimal TinReductionPercentage { get; set; }
    [Range(0, double.MaxValue)] public decimal MonthlyCost { get; set; }
    [Range(0, double.MaxValue)] public decimal YearlyCost { get; set; }
    [Range(0, double.MaxValue)] public decimal OneTimeCost { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsAccepted { get; set; }
    public int? DurationYears { get; set; }
    public string? Notes { get; set; }
}

public class StrategyParametersDto
{
    [Range(0, 100)] public decimal ExpectedAnnualReturnConservative { get; set; } = 3m;
    [Range(0, 100)] public decimal ExpectedAnnualReturnBase { get; set; } = 6m;
    [Range(0, 100)] public decimal ExpectedAnnualReturnOptimistic { get; set; } = 9m;
    public bool UseNetReturns { get; set; } = true;
    public decimal? GrossExpectedReturn { get; set; }
    public decimal? NetExpectedReturn { get; set; }
    [Range(1, 50)] public int AnalysisHorizonYears { get; set; } = 20;
    [Range(0, double.MaxValue)] public decimal MinimumLiquidityCushion { get; set; } = 10000m;
    public RiskProfile RiskProfile { get; set; } = RiskProfile.Balanced;
    [Range(0, double.MaxValue)] public decimal AdditionalCapitalToPreserve { get; set; }
}

// ──── Calculation Request DTOs ────

public class SaleLiquidityRequest
{
    [Required] public SaleDataDto Sale { get; set; } = new();
}

public class PurchaseCostRequest
{
    [Required] public PurchaseDataDto Purchase { get; set; } = new();
    [Range(0, double.MaxValue)] public decimal RealAvailableCash { get; set; }
}

public class DebtCapacityRequest
{
    [Required] public DebtCapacityDataDto DebtCapacity { get; set; } = new();
}

public class MortgageRequest
{
    [Range(0.01, double.MaxValue)] public decimal Principal { get; set; }
    [Range(0, 20)] public decimal AnnualTinPercentage { get; set; }
    [Range(1, 50)] public int TermYears { get; set; }
    [Range(0, double.MaxValue)] public decimal MonthlyNetSalary { get; set; }
    [Range(0, double.MaxValue)] public decimal MonthlyOutstandingLoanPayments { get; set; }
}

public class BankOfferEvaluationRequest
{
    [Required] public BankOfferDto BankOffer { get; set; } = new();
    [Range(0.01, double.MaxValue)] public decimal MortgagePrincipal { get; set; }
    [Range(1, 50)] public int ReferenceTermYears { get; set; } = 25;
    [Range(0, double.MaxValue)] public decimal MonthlyNetSalary { get; set; }
    [Range(0, double.MaxValue)] public decimal MonthlyOutstandingLoanPayments { get; set; }
}

public class StrategyComparisonRequest
{
    [Range(0, double.MaxValue)] public decimal RealAvailableCash { get; set; }
    [Range(0.01, double.MaxValue)] public decimal PurchasePrice { get; set; }
    [Range(0, double.MaxValue)] public decimal MaxMortgageAmount { get; set; }
    [Range(0, double.MaxValue)] public decimal TotalPurchaseCostsExcludingEntry { get; set; }
    [Range(0, 20)] public decimal MortgageTin { get; set; }
    [Range(1, 50)] public int TermYears { get; set; }
    [Range(0, double.MaxValue)] public decimal MonthlyNetSalary { get; set; }
    [Range(0, double.MaxValue)] public decimal MonthlyOutstandingLoanPayments { get; set; }
    [Range(0, double.MaxValue)] public decimal TotalAcceptedBonusCost { get; set; }
    [Required] public StrategyParametersDto StrategyParameters { get; set; } = new();
}

// ──── Response DTOs ────

public class ScenarioResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SaleDataDto Sale { get; set; } = new();
    public PurchaseDataDto Purchase { get; set; } = new();
    public DebtCapacityDataDto DebtCapacity { get; set; } = new();
    public List<BankOfferDto> Banks { get; set; } = [];
    public StrategyParametersDto StrategyParameters { get; set; } = new();
    public AnalysisResultResponse? LastResult { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ScenarioListItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool HasResults { get; set; }
}

public class AnalysisResultResponse
{
    public SaleLiquidityResultDto SaleLiquidity { get; set; } = new();
    public PurchaseCostResultDto PurchaseCosts { get; set; } = new();
    public DebtCapacityResultDto DebtCapacity { get; set; } = new();
    public List<BankOfferResultDto> BankResults { get; set; } = [];
    public StrategyComparisonResultDto? StrategyComparison { get; set; }
    public DateTime CalculatedAt { get; set; }
}

public class SaleLiquidityResultDto
{
    public decimal SalePrice { get; set; }
    public decimal SaleRelatedCosts { get; set; }
    public decimal OutstandingMortgageDebt { get; set; }
    public decimal MunicipalCapitalGainsTax { get; set; }
    public decimal ExtraordinaryCosts { get; set; }
    public decimal NetSaleLiquidity { get; set; }
    public decimal CurrentCashBalance { get; set; }
    public decimal RealAvailableCash { get; set; }
}

public class PurchaseCostResultDto
{
    public decimal ItpAmount { get; set; }
    public decimal MaxMortgageAmount { get; set; }
    public decimal EntryPayment { get; set; }
    public decimal NotaryCosts { get; set; }
    public decimal AdministrativeCosts { get; set; }
    public decimal AppraisalCosts { get; set; }
    public decimal AgencyCosts { get; set; }
    public decimal OtherCosts { get; set; }
    public decimal TotalCashNeeded { get; set; }
    public decimal RemainingLiquidity { get; set; }
    public bool IsViable { get; set; }
}

public class DebtCapacityResultDto
{
    public decimal MaxMonthlyPaymentCapacity { get; set; }
}

public class MortgageResultDto
{
    public decimal Principal { get; set; }
    public decimal TinPercentage { get; set; }
    public int TermYears { get; set; }
    public decimal MonthlyPayment { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal ResultingDebtRatio { get; set; }
    public decimal RemainingMonthlyFreeCash { get; set; }
}

public class BonusEvaluationDto
{
    public string BonusId { get; set; } = string.Empty;
    public string BonusName { get; set; } = string.Empty;
    public decimal InterestSavings { get; set; }
    public decimal TotalBonusCost { get; set; }
    public decimal NetGainOrLoss { get; set; }
    public bool IsWorthIt { get; set; }
}

public class BankOfferResultDto
{
    public string BankId { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public decimal BaseTin { get; set; }
    public decimal MaxTheoreticalDiscountedTin { get; set; }
    public decimal FinalRealTin { get; set; }
    public List<MortgageResultDto> MortgagesByTerm { get; set; } = [];
    public decimal TotalAcceptedBonusCost { get; set; }
    public List<BonusEvaluationDto> BonusEvaluations { get; set; } = [];
    public decimal RealGlobalCost { get; set; }
    public string Recommendation { get; set; } = string.Empty;
}

public class StrategyResultDto
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

public class StrategyComparisonResultDto
{
    public StrategyResultDto AmortizeFaster { get; set; } = new();
    public StrategyResultDto MaintainCapital { get; set; } = new();
    public decimal NetDifferenceConservative { get; set; }
    public decimal NetDifferenceBase { get; set; }
    public decimal NetDifferenceOptimistic { get; set; }
    public decimal WeightedNetDifference { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}
