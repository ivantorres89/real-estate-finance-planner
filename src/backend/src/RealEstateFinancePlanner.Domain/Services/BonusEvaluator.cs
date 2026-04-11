using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Enums;

namespace RealEstateFinancePlanner.Domain.Services;

public static class BonusEvaluator
{
    /// <summary>
    /// Evaluates all bonuses for a bank offer, determining which ones are worth accepting.
    /// A bonus is worth it if the total interest savings over the mortgage term exceed
    /// the total cost of the bonus over its active period.
    /// </summary>
    public static BankOfferResult Evaluate(
        BankOffer bankOffer,
        decimal mortgagePrincipal,
        int referenceTermYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments)
    {
        ArgumentNullException.ThrowIfNull(bankOffer);

        if (mortgagePrincipal <= 0)
            throw new ArgumentException("Mortgage principal must be positive.", nameof(mortgagePrincipal));

        decimal baseTin = bankOffer.BaseTinPercentage;

        // Calculate total possible TIN reduction from all bonuses
        decimal maxTotalReduction = bankOffer.Bonuses.Sum(b => b.TinReductionPercentage);
        decimal maxTheoreticalTin = Math.Max(0m, baseTin - maxTotalReduction);

        // Calculate accepted bonus TIN reduction
        var acceptedBonuses = bankOffer.Bonuses.Where(b => b.IsAccepted || b.IsMandatory).ToList();
        decimal acceptedReduction = acceptedBonuses.Sum(b => b.TinReductionPercentage);
        decimal finalRealTin = Math.Max(0m, baseTin - acceptedReduction);

        // Calculate base mortgage for reference
        var baseMortgage = MortgageCalculator.Calculate(
            mortgagePrincipal, baseTin, referenceTermYears,
            monthlyNetSalary, monthlyOutstandingLoanPayments);

        // Evaluate each bonus individually
        var bonusEvaluations = new List<BonusEvaluation>();
        foreach (var bonus in bankOffer.Bonuses)
        {
            var evaluation = EvaluateSingleBonus(
                bonus, baseTin, mortgagePrincipal, referenceTermYears,
                monthlyNetSalary, monthlyOutstandingLoanPayments);
            bonusEvaluations.Add(evaluation);
        }

        // Calculate accepted bonus total cost
        decimal totalAcceptedBonusCost = CalculateTotalBonusCost(acceptedBonuses, referenceTermYears);

        // Calculate mortgages for all requested terms
        var mortgagesByTerm = bankOffer.MortgageTermsYears
            .Select(term => MortgageCalculator.Calculate(
                mortgagePrincipal, finalRealTin, term,
                monthlyNetSalary, monthlyOutstandingLoanPayments))
            .ToList();

        // Real global cost for the reference term
        var referenceMortgage = MortgageCalculator.Calculate(
            mortgagePrincipal, finalRealTin, referenceTermYears,
            monthlyNetSalary, monthlyOutstandingLoanPayments);

        decimal realGlobalCost = mortgagePrincipal + referenceMortgage.TotalInterest + totalAcceptedBonusCost;

        return new BankOfferResult
        {
            BankId = bankOffer.Id,
            BankName = bankOffer.BankName,
            BaseTin = baseTin,
            MaxTheoreticalDiscountedTin = maxTheoreticalTin,
            FinalRealTin = finalRealTin,
            MortgagesByTerm = mortgagesByTerm,
            TotalAcceptedBonusCost = totalAcceptedBonusCost,
            BonusEvaluations = bonusEvaluations,
            RealGlobalCost = realGlobalCost,
            Recommendation = BankRecommendationLevel.Recommended // Set by comparison later
        };
    }

    /// <summary>
    /// Assigns recommendation levels to bank results by comparing their real global costs.
    /// </summary>
    public static void AssignRecommendations(
        List<BankOfferResult> bankResults,
        decimal maxDebtRatioPercentage,
        int referenceTermYears)
    {
        if (bankResults.Count == 0) return;

        decimal bestCost = bankResults.Min(b => b.RealGlobalCost);

        foreach (var result in bankResults)
        {
            // Check debt ratio for reference term
            var referenceMortgage = result.MortgagesByTerm
                .FirstOrDefault(m => m.TermYears == referenceTermYears);

            bool debtRatioExceeded = referenceMortgage != null
                && referenceMortgage.ResultingDebtRatio > maxDebtRatioPercentage;

            bool debtRatioApproaching = referenceMortgage != null
                && referenceMortgage.ResultingDebtRatio > maxDebtRatioPercentage - 5m;

            if (debtRatioExceeded)
            {
                result.Recommendation = BankRecommendationLevel.NotWorthIt;
            }
            else if (result.RealGlobalCost <= bestCost * 1.02m && !debtRatioApproaching)
            {
                result.Recommendation = BankRecommendationLevel.Recommended;
            }
            else if (result.RealGlobalCost <= bestCost * 1.05m || debtRatioApproaching)
            {
                result.Recommendation = BankRecommendationLevel.Questionable;
            }
            else
            {
                result.Recommendation = BankRecommendationLevel.NotWorthIt;
            }
        }
    }

    private static BonusEvaluation EvaluateSingleBonus(
        Bonus bonus,
        decimal baseTin,
        decimal principal,
        int termYears,
        decimal monthlyNetSalary,
        decimal monthlyOutstandingLoanPayments)
    {
        // Mortgage WITHOUT this particular bonus reduction
        var mortgageWithout = MortgageCalculator.Calculate(
            principal, baseTin, termYears,
            monthlyNetSalary, monthlyOutstandingLoanPayments);

        // Mortgage WITH this particular bonus reduction
        decimal reducedTin = Math.Max(0m, baseTin - bonus.TinReductionPercentage);
        var mortgageWith = MortgageCalculator.Calculate(
            principal, reducedTin, termYears,
            monthlyNetSalary, monthlyOutstandingLoanPayments);

        decimal interestSavings = mortgageWithout.TotalInterest - mortgageWith.TotalInterest;

        int effectiveDuration = bonus.DurationYears ?? termYears;
        decimal totalBonusCost = CalculateSingleBonusCost(bonus, effectiveDuration);

        decimal netGainOrLoss = interestSavings - totalBonusCost;

        return new BonusEvaluation
        {
            BonusId = bonus.Id,
            BonusName = bonus.Name,
            InterestSavings = Math.Round(interestSavings, 2),
            TotalBonusCost = Math.Round(totalBonusCost, 2),
            NetGainOrLoss = Math.Round(netGainOrLoss, 2),
            IsWorthIt = netGainOrLoss > 0
        };
    }

    private static decimal CalculateSingleBonusCost(Bonus bonus, int durationYears)
    {
        decimal monthlyCostTotal = bonus.MonthlyCost * 12m * durationYears;
        decimal yearlyCostTotal = bonus.YearlyCost * durationYears;
        return monthlyCostTotal + yearlyCostTotal + bonus.OneTimeCost;
    }

    private static decimal CalculateTotalBonusCost(List<Bonus> bonuses, int mortgageTermYears)
    {
        decimal total = 0m;
        foreach (var bonus in bonuses)
        {
            int effectiveDuration = bonus.DurationYears ?? mortgageTermYears;
            total += CalculateSingleBonusCost(bonus, effectiveDuration);
        }
        return total;
    }
}
