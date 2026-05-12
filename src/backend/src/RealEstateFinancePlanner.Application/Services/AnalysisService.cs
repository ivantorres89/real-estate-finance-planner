using RealEstateFinancePlanner.Application.Interfaces;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Services;

namespace RealEstateFinancePlanner.Application.Services;

public class AnalysisService : IAnalysisService
{
    public SaleLiquidityResult CalculateSaleLiquidity(SaleData sale)
        => SaleLiquidityCalculator.Calculate(sale);

    public PurchaseCostResult CalculatePurchaseCosts(
        PurchaseData purchase,
        decimal realAvailableCashA,
        decimal realAvailableCashB)
        => PurchaseCostCalculator.Calculate(purchase, realAvailableCashA, realAvailableCashB);

    public DebtCapacityResult CalculateDebtCapacity(DebtCapacityData debtData)
        => DebtCapacityCalculator.Calculate(debtData);

    public MortgageCalculationResult CalculateMortgage(
        decimal principal,
        decimal annualTinPercentage,
        int termYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments)
        => MortgageCalculator.Calculate(principal, annualTinPercentage, termYears, monthlyNetSalary, monthlyOutstandingLoanPayments);

    public BankOfferResult EvaluateBankOffer(
        BankOffer bankOffer,
        decimal mortgagePrincipal,
        int referenceTermYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments)
        => BonusEvaluator.Evaluate(bankOffer, mortgagePrincipal, referenceTermYears, monthlyNetSalary, monthlyOutstandingLoanPayments);

    public StrategyComparisonResult CompareStrategies(
        decimal realAvailableCashA,
        decimal officialPurchasePriceA,
        decimal maxMortgageAmount,
        decimal totalPurchaseCostsExcludingEntryA,
        decimal mortgageTin,
        int termYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments,
        decimal totalAcceptedBonusCost,
        StrategyParameters parameters)
        => StrategyRecommendationEngine.Compare(
            realAvailableCashA, officialPurchasePriceA, maxMortgageAmount,
            totalPurchaseCostsExcludingEntryA, mortgageTin, termYears,
            monthlyNetSalary, monthlyOutstandingLoanPayments,
            totalAcceptedBonusCost, parameters);

    public AnalysisResult RunFullAnalysis(Scenario scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        // 1. Sale liquidity
        var saleLiquidity = CalculateSaleLiquidity(scenario.Sale);

        // 2. Purchase costs
        var purchaseCosts = CalculatePurchaseCosts(
            scenario.Purchase,
            saleLiquidity.RealAvailableCashA,
            saleLiquidity.RealAvailableCashB);

        // 3. Debt capacity
        var debtCapacity = CalculateDebtCapacity(scenario.DebtCapacity);

        // 4. Evaluate each bank offer
        var bankResults = new List<BankOfferResult>();
        int defaultTermYears = 25;

        foreach (var bank in scenario.Banks)
        {
            int referenceTermYears = bank.MortgageTermsYears.Count > 0
                ? bank.MortgageTermsYears[bank.MortgageTermsYears.Count / 2]
                : defaultTermYears;

            var bankResult = EvaluateBankOffer(
                bank,
                purchaseCosts.MaxMortgageAmount,
                referenceTermYears,
                scenario.DebtCapacity.MonthlyNetSalary,
                scenario.DebtCapacity.MonthlyOutstandingLoanPayments);

            bankResults.Add(bankResult);
        }

        // 5. Assign bank recommendations
        if (bankResults.Count > 0)
        {
            // When multiple terms tie in frequency (e.g., a single bank offering many terms),
            // pick the median of the tied terms rather than the first-encountered shortest one.
            var termCounts = bankResults
                .SelectMany(b => b.MortgagesByTerm.Select(m => m.TermYears))
                .GroupBy(t => t)
                .Select(g => (Term: g.Key, Count: g.Count()))
                .OrderByDescending(x => x.Count)
                .ToList();
            int maxCount = termCounts[0].Count;
            var tiedTerms = termCounts
                .Where(x => x.Count == maxCount)
                .Select(x => x.Term)
                .OrderBy(t => t)
                .ToList();
            int commonTerm = tiedTerms[tiedTerms.Count / 2];

            BonusEvaluator.AssignRecommendations(
                bankResults,
                scenario.DebtCapacity.MaxDebtRatioPercentage,
                commonTerm);
        }

        // 6. Strategy comparison (use best bank's final TIN). Strategy operates only on
        // the A-side: B-side cash is not freely investable capital (it cannot be moved
        // into a brokerage account without justification), so the A/B comparison
        // ignores B.
        StrategyComparisonResult? strategyComparison = null;
        if (bankResults.Count > 0)
        {
            var bestBank = bankResults.OrderBy(b => b.RealGlobalCost).First();
            int strategyTerm = bestBank.MortgagesByTerm.Count > 0
                ? bestBank.MortgagesByTerm[bestBank.MortgagesByTerm.Count / 2].TermYears
                : defaultTermYears;

            decimal totalCostsExcludingEntryA = purchaseCosts.TotalCashNeededA - purchaseCosts.EntryPaymentA;

            strategyComparison = CompareStrategies(
                saleLiquidity.RealAvailableCashA,
                scenario.Purchase.OfficialPurchasePriceA,
                purchaseCosts.MaxMortgageAmount,
                totalCostsExcludingEntryA,
                bestBank.FinalRealTin,
                strategyTerm,
                scenario.DebtCapacity.MonthlyNetSalary,
                scenario.DebtCapacity.MonthlyOutstandingLoanPayments,
                bestBank.TotalAcceptedBonusCost,
                scenario.StrategyParameters);
        }

        return new AnalysisResult
        {
            SaleLiquidity = saleLiquidity,
            PurchaseCosts = purchaseCosts,
            DebtCapacity = debtCapacity,
            BankResults = bankResults,
            StrategyComparison = strategyComparison,
            CalculatedAt = DateTime.UtcNow,
        };
    }
}
