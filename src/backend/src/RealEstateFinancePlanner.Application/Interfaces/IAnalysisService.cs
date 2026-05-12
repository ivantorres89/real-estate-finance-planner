using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Application.Interfaces;

public interface IAnalysisService
{
    SaleLiquidityResult CalculateSaleLiquidity(SaleData sale);

    PurchaseCostResult CalculatePurchaseCosts(
        PurchaseData purchase,
        decimal realAvailableCashA,
        decimal realAvailableCashB);

    DebtCapacityResult CalculateDebtCapacity(DebtCapacityData debtData);

    MortgageCalculationResult CalculateMortgage(
        decimal principal,
        decimal annualTinPercentage,
        int termYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments);

    BankOfferResult EvaluateBankOffer(
        BankOffer bankOffer,
        decimal mortgagePrincipal,
        int referenceTermYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments);

    StrategyComparisonResult CompareStrategies(
        decimal realAvailableCashA,
        decimal officialPurchasePriceA,
        decimal maxMortgageAmount,
        decimal totalPurchaseCostsExcludingEntryA,
        decimal mortgageTin,
        int termYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments,
        decimal totalAcceptedBonusCost,
        StrategyParameters parameters);

    AnalysisResult RunFullAnalysis(Scenario scenario);
}
