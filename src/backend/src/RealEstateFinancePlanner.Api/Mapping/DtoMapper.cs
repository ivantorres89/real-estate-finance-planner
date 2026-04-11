using RealEstateFinancePlanner.Api.Dtos;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Enums;

namespace RealEstateFinancePlanner.Api.Mapping;

public static class DtoMapper
{
    // ──── Domain → DTO ────

    public static ScenarioResponse ToResponse(this Scenario entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Sale = entity.Sale.ToDto(),
        Purchase = entity.Purchase.ToDto(),
        DebtCapacity = entity.DebtCapacity.ToDto(),
        Banks = entity.Banks.Select(b => b.ToDto()).ToList(),
        StrategyParameters = entity.StrategyParameters.ToDto(),
        LastResult = entity.LastResult?.ToDto(),
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
    };

    public static ScenarioListItem ToListItem(this Scenario entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        HasResults = entity.LastResult != null,
    };

    public static SaleDataDto ToDto(this SaleData e) => new()
    {
        SalePrice = e.SalePrice,
        SaleRelatedCosts = e.SaleRelatedCosts,
        OutstandingMortgageDebt = e.OutstandingMortgageDebt,
        CurrentCashBalance = e.CurrentCashBalance,
        MunicipalCapitalGainsTax = e.MunicipalCapitalGainsTax,
        ExtraordinaryCosts = e.ExtraordinaryCosts,
    };

    public static PurchaseDataDto ToDto(this PurchaseData e) => new()
    {
        PurchasePrice = e.PurchasePrice,
        DeedPrice = e.DeedPrice,
        FinanceablePercentage = e.FinanceablePercentage,
        NotaryCosts = e.NotaryCosts,
        AdministrativeCosts = e.AdministrativeCosts,
        AppraisalCosts = e.AppraisalCosts,
        AgencyCosts = e.AgencyCosts,
        OtherCosts = e.OtherCosts,
        ApplyReducedItp = e.ApplyReducedItp,
        IsMainResidence = e.IsMainResidence,
        BuyerAge = e.BuyerAge,
    };

    public static DebtCapacityDataDto ToDto(this DebtCapacityData e) => new()
    {
        MonthlyNetSalary = e.MonthlyNetSalary,
        MonthlyOutstandingLoanPayments = e.MonthlyOutstandingLoanPayments,
        MaxDebtRatioPercentage = e.MaxDebtRatioPercentage,
    };

    public static BankOfferDto ToDto(this BankOffer e) => new()
    {
        Id = e.Id,
        BankName = e.BankName,
        BaseTinPercentage = e.BaseTinPercentage,
        Bonuses = e.Bonuses.Select(b => b.ToDto()).ToList(),
        MortgageTermsYears = e.MortgageTermsYears,
    };

    public static BonusDto ToDto(this Bonus e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Category = e.Category,
        TinReductionPercentage = e.TinReductionPercentage,
        MonthlyCost = e.MonthlyCost,
        YearlyCost = e.YearlyCost,
        OneTimeCost = e.OneTimeCost,
        IsMandatory = e.IsMandatory,
        IsAccepted = e.IsAccepted,
        DurationYears = e.DurationYears,
        Notes = e.Notes,
    };

    public static StrategyParametersDto ToDto(this StrategyParameters e) => new()
    {
        ExpectedAnnualReturnConservative = e.ExpectedAnnualReturnConservative,
        ExpectedAnnualReturnBase = e.ExpectedAnnualReturnBase,
        ExpectedAnnualReturnOptimistic = e.ExpectedAnnualReturnOptimistic,
        UseNetReturns = e.UseNetReturns,
        GrossExpectedReturn = e.GrossExpectedReturn,
        NetExpectedReturn = e.NetExpectedReturn,
        AnalysisHorizonYears = e.AnalysisHorizonYears,
        MinimumLiquidityCushion = e.MinimumLiquidityCushion,
        RiskProfile = e.RiskProfile,
        AdditionalCapitalToPreserve = e.AdditionalCapitalToPreserve,
    };

    public static AnalysisResultResponse ToDto(this AnalysisResult e) => new()
    {
        SaleLiquidity = new SaleLiquidityResultDto
        {
            NetSaleLiquidity = e.SaleLiquidity.NetSaleLiquidity,
            RealAvailableCash = e.SaleLiquidity.RealAvailableCash,
        },
        PurchaseCosts = new PurchaseCostResultDto
        {
            ItpAmount = e.PurchaseCosts.ItpAmount,
            MaxMortgageAmount = e.PurchaseCosts.MaxMortgageAmount,
            EntryPayment = e.PurchaseCosts.EntryPayment,
            TotalCashNeeded = e.PurchaseCosts.TotalCashNeeded,
            RemainingLiquidity = e.PurchaseCosts.RemainingLiquidity,
            IsViable = e.PurchaseCosts.IsViable,
        },
        DebtCapacity = new DebtCapacityResultDto
        {
            MaxMonthlyPaymentCapacity = e.DebtCapacity.MaxMonthlyPaymentCapacity,
        },
        BankResults = e.BankResults.Select(b => b.ToDto()).ToList(),
        StrategyComparison = e.StrategyComparison?.ToDto(),
        CalculatedAt = e.CalculatedAt,
    };

    public static BankOfferResultDto ToDto(this BankOfferResult e) => new()
    {
        BankId = e.BankId,
        BankName = e.BankName,
        BaseTin = e.BaseTin,
        MaxTheoreticalDiscountedTin = e.MaxTheoreticalDiscountedTin,
        FinalRealTin = e.FinalRealTin,
        MortgagesByTerm = e.MortgagesByTerm.Select(m => m.ToDto()).ToList(),
        TotalAcceptedBonusCost = e.TotalAcceptedBonusCost,
        BonusEvaluations = e.BonusEvaluations.Select(be => be.ToDto()).ToList(),
        RealGlobalCost = e.RealGlobalCost,
        Recommendation = e.Recommendation.ToString(),
    };

    public static MortgageResultDto ToDto(this MortgageCalculationResult e) => new()
    {
        Principal = e.Principal,
        TinPercentage = e.TinPercentage,
        TermYears = e.TermYears,
        MonthlyPayment = e.MonthlyPayment,
        TotalPaid = e.TotalPaid,
        TotalInterest = e.TotalInterest,
        ResultingDebtRatio = e.ResultingDebtRatio,
        RemainingMonthlyFreeCash = e.RemainingMonthlyFreeCash,
    };

    public static BonusEvaluationDto ToDto(this BonusEvaluation e) => new()
    {
        BonusId = e.BonusId,
        BonusName = e.BonusName,
        InterestSavings = e.InterestSavings,
        TotalBonusCost = e.TotalBonusCost,
        NetGainOrLoss = e.NetGainOrLoss,
        IsWorthIt = e.IsWorthIt,
    };

    public static StrategyComparisonResultDto ToDto(this StrategyComparisonResult e) => new()
    {
        AmortizeFaster = e.AmortizeFaster.ToDto(),
        MaintainCapital = e.MaintainCapital.ToDto(),
        NetDifferenceConservative = e.NetDifferenceConservative,
        NetDifferenceBase = e.NetDifferenceBase,
        NetDifferenceOptimistic = e.NetDifferenceOptimistic,
        WeightedNetDifference = e.WeightedNetDifference,
        Recommendation = e.Recommendation.ToString(),
        Explanation = e.Explanation,
    };

    public static StrategyResultDto ToDto(this StrategyResult e) => new()
    {
        EntryPayment = e.EntryPayment,
        MortgagePrincipal = e.MortgagePrincipal,
        MonthlyPayment = e.MonthlyPayment,
        TotalInterest = e.TotalInterest,
        TotalBonusCost = e.TotalBonusCost,
        RemainingLiquidity = e.RemainingLiquidity,
        DebtRatio = e.DebtRatio,
        TotalFinancialCost = e.TotalFinancialCost,
        FutureValueConservative = e.FutureValueConservative,
        FutureValueBase = e.FutureValueBase,
        FutureValueOptimistic = e.FutureValueOptimistic,
    };

    // ──── DTO → Domain ────

    public static Scenario ToEntity(this CreateScenarioRequest dto) => new()
    {
        Name = dto.Name,
        Sale = dto.Sale.ToEntity(),
        Purchase = dto.Purchase.ToEntity(),
        DebtCapacity = dto.DebtCapacity.ToEntity(),
        Banks = dto.Banks.Select(b => b.ToEntity()).ToList(),
        StrategyParameters = dto.StrategyParameters.ToEntity(),
    };

    public static SaleData ToEntity(this SaleDataDto dto) => new()
    {
        SalePrice = dto.SalePrice,
        SaleRelatedCosts = dto.SaleRelatedCosts,
        OutstandingMortgageDebt = dto.OutstandingMortgageDebt,
        CurrentCashBalance = dto.CurrentCashBalance,
        MunicipalCapitalGainsTax = dto.MunicipalCapitalGainsTax,
        ExtraordinaryCosts = dto.ExtraordinaryCosts,
    };

    public static PurchaseData ToEntity(this PurchaseDataDto dto) => new()
    {
        PurchasePrice = dto.PurchasePrice,
        DeedPrice = dto.DeedPrice,
        FinanceablePercentage = dto.FinanceablePercentage,
        NotaryCosts = dto.NotaryCosts,
        AdministrativeCosts = dto.AdministrativeCosts,
        AppraisalCosts = dto.AppraisalCosts,
        AgencyCosts = dto.AgencyCosts,
        OtherCosts = dto.OtherCosts,
        ApplyReducedItp = dto.ApplyReducedItp,
        IsMainResidence = dto.IsMainResidence,
        BuyerAge = dto.BuyerAge,
    };

    public static DebtCapacityData ToEntity(this DebtCapacityDataDto dto) => new()
    {
        MonthlyNetSalary = dto.MonthlyNetSalary,
        MonthlyOutstandingLoanPayments = dto.MonthlyOutstandingLoanPayments,
        MaxDebtRatioPercentage = dto.MaxDebtRatioPercentage,
    };

    public static BankOffer ToEntity(this BankOfferDto dto) => new()
    {
        Id = dto.Id ?? Guid.NewGuid().ToString(),
        BankName = dto.BankName,
        BaseTinPercentage = dto.BaseTinPercentage,
        Bonuses = dto.Bonuses.Select(b => b.ToEntity()).ToList(),
        MortgageTermsYears = dto.MortgageTermsYears,
    };

    public static Bonus ToEntity(this BonusDto dto) => new()
    {
        Id = dto.Id ?? Guid.NewGuid().ToString(),
        Name = dto.Name,
        Category = dto.Category,
        TinReductionPercentage = dto.TinReductionPercentage,
        MonthlyCost = dto.MonthlyCost,
        YearlyCost = dto.YearlyCost,
        OneTimeCost = dto.OneTimeCost,
        IsMandatory = dto.IsMandatory,
        IsAccepted = dto.IsAccepted,
        DurationYears = dto.DurationYears,
        Notes = dto.Notes,
    };

    public static StrategyParameters ToEntity(this StrategyParametersDto dto) => new()
    {
        ExpectedAnnualReturnConservative = dto.ExpectedAnnualReturnConservative,
        ExpectedAnnualReturnBase = dto.ExpectedAnnualReturnBase,
        ExpectedAnnualReturnOptimistic = dto.ExpectedAnnualReturnOptimistic,
        UseNetReturns = dto.UseNetReturns,
        GrossExpectedReturn = dto.GrossExpectedReturn,
        NetExpectedReturn = dto.NetExpectedReturn,
        AnalysisHorizonYears = dto.AnalysisHorizonYears,
        MinimumLiquidityCushion = dto.MinimumLiquidityCushion,
        RiskProfile = dto.RiskProfile,
        AdditionalCapitalToPreserve = dto.AdditionalCapitalToPreserve,
    };
}
