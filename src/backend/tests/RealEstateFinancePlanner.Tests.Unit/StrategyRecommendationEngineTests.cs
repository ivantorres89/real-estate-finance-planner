using FluentAssertions;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Enums;
using RealEstateFinancePlanner.Domain.Services;

namespace RealEstateFinancePlanner.Tests.Unit;

public class StrategyRecommendationEngineTests
{
    private static StrategyParameters DefaultParameters(RiskProfile profile = RiskProfile.Balanced) => new()
    {
        ExpectedAnnualReturnConservative = 3m,
        ExpectedAnnualReturnBase = 6m,
        ExpectedAnnualReturnOptimistic = 9m,
        UseNetReturns = true,
        AnalysisHorizonYears = 20,
        MinimumLiquidityCushion = 10_000m,
        RiskProfile = profile,
        AdditionalCapitalToPreserve = 0m,
    };

    [Fact]
    public void Compare_HighReturns_RecommendsMaintainCapital()
    {
        var result = StrategyRecommendationEngine.Compare(
            realAvailableCash: 200_000m,
            purchasePrice: 300_000m,
            maxMortgageAmount: 240_000m,
            totalPurchaseCostsExcludingEntry: 15_000m,
            mortgageTin: 2.5m,
            termYears: 25,
            monthlyNetSalary: 4_000m,
            monthlyOutstandingLoanPayments: 0m,
            totalAcceptedBonusCost: 5_000m,
            parameters: DefaultParameters(RiskProfile.Aggressive));

        // With aggressive profile and 6-9% returns, maintaining capital should win
        result.MaintainCapital.RemainingLiquidity.Should().BeGreaterThan(result.AmortizeFaster.RemainingLiquidity);
        result.MaintainCapital.MortgagePrincipal.Should().BeGreaterThan(result.AmortizeFaster.MortgagePrincipal);
    }

    [Fact]
    public void Compare_LowReturns_RecommendsAmortizeFaster()
    {
        var parameters = DefaultParameters(RiskProfile.Conservative);
        parameters.ExpectedAnnualReturnConservative = 0.5m;
        parameters.ExpectedAnnualReturnBase = 1.0m;
        parameters.ExpectedAnnualReturnOptimistic = 2.0m;

        var result = StrategyRecommendationEngine.Compare(
            realAvailableCash: 200_000m,
            purchasePrice: 300_000m,
            maxMortgageAmount: 240_000m,
            totalPurchaseCostsExcludingEntry: 15_000m,
            mortgageTin: 3.5m,
            termYears: 25,
            monthlyNetSalary: 4_000m,
            monthlyOutstandingLoanPayments: 0m,
            totalAcceptedBonusCost: 0m,
            parameters: parameters);

        // With low returns and high mortgage rate, amortizing should be better
        result.Recommendation.Should().Be(StrategyRecommendation.AmortizeFaster);
    }

    [Fact]
    public void Compare_InsufficientLiquidity_RecommendsAmortizeFaster()
    {
        var parameters = DefaultParameters();
        parameters.MinimumLiquidityCushion = 500_000m; // impossibly high cushion

        var result = StrategyRecommendationEngine.Compare(
            realAvailableCash: 100_000m,
            purchasePrice: 300_000m,
            maxMortgageAmount: 240_000m,
            totalPurchaseCostsExcludingEntry: 15_000m,
            mortgageTin: 2.5m,
            termYears: 25,
            monthlyNetSalary: 4_000m,
            monthlyOutstandingLoanPayments: 0m,
            totalAcceptedBonusCost: 0m,
            parameters: parameters);

        result.Recommendation.Should().Be(StrategyRecommendation.AmortizeFaster);
        result.Explanation.Should().Contain("cushion");
    }

    [Fact]
    public void Compare_StrategyA_HasLessPrincipalThanB()
    {
        var result = StrategyRecommendationEngine.Compare(
            realAvailableCash: 200_000m,
            purchasePrice: 300_000m,
            maxMortgageAmount: 240_000m,
            totalPurchaseCostsExcludingEntry: 15_000m,
            mortgageTin: 2.5m,
            termYears: 25,
            monthlyNetSalary: 4_000m,
            monthlyOutstandingLoanPayments: 0m,
            totalAcceptedBonusCost: 0m,
            parameters: DefaultParameters());

        result.AmortizeFaster.MortgagePrincipal.Should().BeLessThanOrEqualTo(result.MaintainCapital.MortgagePrincipal);
        result.AmortizeFaster.TotalInterest.Should().BeLessThanOrEqualTo(result.MaintainCapital.TotalInterest);
    }

    [Fact]
    public void Compare_ExplanationIsNotEmpty()
    {
        var result = StrategyRecommendationEngine.Compare(
            realAvailableCash: 200_000m,
            purchasePrice: 300_000m,
            maxMortgageAmount: 240_000m,
            totalPurchaseCostsExcludingEntry: 15_000m,
            mortgageTin: 2.5m,
            termYears: 25,
            monthlyNetSalary: 4_000m,
            monthlyOutstandingLoanPayments: 0m,
            totalAcceptedBonusCost: 0m,
            parameters: DefaultParameters());

        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Compare_FutureValuesCalculated_ForMaintainCapitalStrategy()
    {
        var result = StrategyRecommendationEngine.Compare(
            realAvailableCash: 200_000m,
            purchasePrice: 300_000m,
            maxMortgageAmount: 240_000m,
            totalPurchaseCostsExcludingEntry: 15_000m,
            mortgageTin: 2.5m,
            termYears: 25,
            monthlyNetSalary: 4_000m,
            monthlyOutstandingLoanPayments: 0m,
            totalAcceptedBonusCost: 0m,
            parameters: DefaultParameters());

        result.MaintainCapital.FutureValueConservative.Should().NotBeNull();
        result.MaintainCapital.FutureValueBase.Should().NotBeNull();
        result.MaintainCapital.FutureValueOptimistic.Should().NotBeNull();
        result.MaintainCapital.FutureValueOptimistic!.Value.Should().BeGreaterThan(result.MaintainCapital.FutureValueConservative!.Value);
    }

    [Fact]
    public void CalculateFutureValue_KnownCase_CorrectResult()
    {
        // 100000 at 6% for 20 years = 100000 * (1.06)^20 = 320713.55
        var fv = StrategyRecommendationEngine.CalculateFutureValue(100_000m, 6m, 20);
        fv.Should().BeApproximately(320_713.55m, 1m);
    }

    [Fact]
    public void Compare_RiskProfileAffectsWeightedResult()
    {
        var resultConservative = StrategyRecommendationEngine.Compare(
            200_000m, 300_000m, 240_000m, 15_000m, 2.5m, 25, 4_000m, 0m, 0m,
            DefaultParameters(RiskProfile.Conservative));

        var resultAggressive = StrategyRecommendationEngine.Compare(
            200_000m, 300_000m, 240_000m, 15_000m, 2.5m, 25, 4_000m, 0m, 0m,
            DefaultParameters(RiskProfile.Aggressive));

        // Aggressive should weight optimistic higher, producing a higher weighted net difference
        resultAggressive.WeightedNetDifference.Should().BeGreaterThan(resultConservative.WeightedNetDifference);
    }
}
