using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Enums;

namespace RealEstateFinancePlanner.Domain.Services;

public static class StrategyRecommendationEngine
{
    private static readonly Dictionary<RiskProfile, (decimal Conservative, decimal Base, decimal Optimistic)> Weights = new()
    {
        [RiskProfile.Conservative] = (0.60m, 0.30m, 0.10m),
        [RiskProfile.Balanced] = (0.25m, 0.50m, 0.25m),
        [RiskProfile.Aggressive] = (0.10m, 0.30m, 0.60m),
    };

    /// <summary>
    /// Compares Strategy A (amortize faster) vs Strategy B (maintain capital) and
    /// produces a recommendation based on the net financial benefit, risk profile,
    /// and liquidity constraints.
    /// </summary>
    public static StrategyComparisonResult Compare(
        decimal realAvailableCash,
        decimal purchasePrice,
        decimal maxMortgageAmount,
        decimal totalPurchaseCostsExcludingEntry,
        decimal mortgageTin,
        int termYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments,
        decimal totalAcceptedBonusCost,
        StrategyParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        // Strategy A: Maximize entry payment (use all available cash minus cushion)
        decimal maxEntryPayment = realAvailableCash
            - totalPurchaseCostsExcludingEntry
            - parameters.MinimumLiquidityCushion
            - parameters.AdditionalCapitalToPreserve;

        decimal strategyAEntry = Math.Max(purchasePrice - maxMortgageAmount, maxEntryPayment);
        strategyAEntry = Math.Max(0m, Math.Min(strategyAEntry, purchasePrice));

        decimal strategyAPrincipal = Math.Max(0m, purchasePrice - strategyAEntry);

        // Strategy B: Minimize entry payment (use minimum required)
        decimal strategyBEntry = Math.Max(0m, purchasePrice - maxMortgageAmount);
        decimal strategyBPrincipal = Math.Max(0m, purchasePrice - strategyBEntry);

        // Calculate mortgages
        var mortgageA = strategyAPrincipal > 0
            ? MortgageCalculator.Calculate(strategyAPrincipal, mortgageTin, termYears, monthlyNetSalary, monthlyOutstandingLoanPayments)
            : CreateZeroMortgage(termYears, mortgageTin);

        var mortgageB = strategyBPrincipal > 0
            ? MortgageCalculator.Calculate(strategyBPrincipal, mortgageTin, termYears, monthlyNetSalary, monthlyOutstandingLoanPayments)
            : CreateZeroMortgage(termYears, mortgageTin);

        // Remaining liquidity after each strategy
        decimal liquidityA = realAvailableCash - strategyAEntry - totalPurchaseCostsExcludingEntry;
        decimal liquidityB = realAvailableCash - strategyBEntry - totalPurchaseCostsExcludingEntry;

        // Capital delta available for reinvestment
        decimal investableCapital = liquidityB - liquidityA;

        // Future value of investable capital under three scenarios
        decimal fvConservative = CalculateFutureValue(investableCapital, parameters.ExpectedAnnualReturnConservative, parameters.AnalysisHorizonYears);
        decimal fvBase = CalculateFutureValue(investableCapital, parameters.ExpectedAnnualReturnBase, parameters.AnalysisHorizonYears);
        decimal fvOptimistic = CalculateFutureValue(investableCapital, parameters.ExpectedAnnualReturnOptimistic, parameters.AnalysisHorizonYears);

        // Additional interest cost of Strategy B vs A
        decimal additionalInterest = mortgageB.TotalInterest - mortgageA.TotalInterest;

        // Net difference: investment return minus additional borrowing cost
        decimal netConservative = fvConservative - investableCapital - additionalInterest;
        decimal netBase = fvBase - investableCapital - additionalInterest;
        decimal netOptimistic = fvOptimistic - investableCapital - additionalInterest;

        // Weighted net difference based on risk profile
        var weights = Weights[parameters.RiskProfile];
        decimal weightedNet = netConservative * weights.Conservative
            + netBase * weights.Base
            + netOptimistic * weights.Optimistic;

        // Determine recommendation
        bool meetsLiquidityCushion = liquidityB >= parameters.MinimumLiquidityCushion;
        bool debtRatioOk = mortgageB.ResultingDebtRatio <= (monthlyNetSalary > 0 ? 35m : 100m);

        StrategyRecommendation recommendation;
        string explanation;

        if (weightedNet > 0 && meetsLiquidityCushion && debtRatioOk)
        {
            recommendation = StrategyRecommendation.MaintainCapital;
            explanation = $"Maintaining capital for reinvestment yields an estimated weighted net benefit of {weightedNet:N2} EUR over {parameters.AnalysisHorizonYears} years. "
                + $"The additional interest cost ({additionalInterest:N2} EUR) is offset by the projected investment returns. "
                + $"Remaining liquidity ({liquidityB:N2} EUR) meets the minimum cushion ({parameters.MinimumLiquidityCushion:N2} EUR).";
        }
        else
        {
            recommendation = StrategyRecommendation.AmortizeFaster;
            var reasons = new List<string>();
            if (weightedNet <= 0)
                reasons.Add($"the weighted projected investment return does not offset the additional interest cost (net: {weightedNet:N2} EUR)");
            if (!meetsLiquidityCushion)
                reasons.Add($"remaining liquidity ({liquidityB:N2} EUR) does not meet the minimum cushion ({parameters.MinimumLiquidityCushion:N2} EUR)");
            if (!debtRatioOk)
                reasons.Add($"the resulting debt ratio ({mortgageB.ResultingDebtRatio:N2}%) exceeds the safe threshold");

            explanation = $"Amortizing faster is recommended because {string.Join("; and ", reasons)}. "
                + $"Total interest saved by choosing Strategy A: {additionalInterest:N2} EUR.";
        }

        return new StrategyComparisonResult
        {
            AmortizeFaster = new StrategyResult
            {
                EntryPayment = strategyAEntry,
                MortgagePrincipal = strategyAPrincipal,
                MonthlyPayment = mortgageA.MonthlyPayment,
                TotalInterest = mortgageA.TotalInterest,
                TotalBonusCost = totalAcceptedBonusCost,
                RemainingLiquidity = liquidityA,
                DebtRatio = mortgageA.ResultingDebtRatio,
                TotalFinancialCost = strategyAPrincipal + mortgageA.TotalInterest + totalAcceptedBonusCost,
            },
            MaintainCapital = new StrategyResult
            {
                EntryPayment = strategyBEntry,
                MortgagePrincipal = strategyBPrincipal,
                MonthlyPayment = mortgageB.MonthlyPayment,
                TotalInterest = mortgageB.TotalInterest,
                TotalBonusCost = totalAcceptedBonusCost,
                RemainingLiquidity = liquidityB,
                DebtRatio = mortgageB.ResultingDebtRatio,
                TotalFinancialCost = strategyBPrincipal + mortgageB.TotalInterest + totalAcceptedBonusCost,
                FutureValueConservative = fvConservative,
                FutureValueBase = fvBase,
                FutureValueOptimistic = fvOptimistic,
            },
            NetDifferenceConservative = Math.Round(netConservative, 2),
            NetDifferenceBase = Math.Round(netBase, 2),
            NetDifferenceOptimistic = Math.Round(netOptimistic, 2),
            WeightedNetDifference = Math.Round(weightedNet, 2),
            Recommendation = recommendation,
            Explanation = explanation,
        };
    }

    /// <summary>
    /// Calculates the future value using annual compound interest.
    /// FV = Capital * (1 + annualReturn/100)^years
    /// </summary>
    internal static decimal CalculateFutureValue(decimal capital, decimal annualReturnPercentage, int years)
    {
        if (capital <= 0 || years <= 0) return capital;

        decimal rate = 1m + annualReturnPercentage / 100m;
        decimal compounded = MortgageCalculator.DecimalPow(rate, years);
        return Math.Round(capital * compounded, 2);
    }

    private static MortgageCalculationResult CreateZeroMortgage(int termYears, decimal tin)
    {
        return new MortgageCalculationResult
        {
            Principal = 0m,
            TinPercentage = tin,
            TermYears = termYears,
            MonthlyPayment = 0m,
            TotalPaid = 0m,
            TotalInterest = 0m,
            ResultingDebtRatio = 0m,
            RemainingMonthlyFreeCash = 0m,
        };
    }
}
