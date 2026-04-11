using FluentAssertions;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Enums;
using RealEstateFinancePlanner.Domain.Services;

namespace RealEstateFinancePlanner.Tests.Unit;

public class BonusEvaluatorTests
{
    private static BankOffer CreateBankOffer(decimal baseTin, params Bonus[] bonuses)
    {
        return new BankOffer
        {
            BankName = "Test Bank",
            BaseTinPercentage = baseTin,
            Bonuses = bonuses.ToList(),
            MortgageTermsYears = [20, 25, 30]
        };
    }

    [Fact]
    public void Evaluate_BonusThatSavesMoreThanItCosts_IsWorthIt()
    {
        var bonus = new Bonus
        {
            Name = "Payroll domiciliation",
            Category = BonusCategory.Payroll,
            TinReductionPercentage = 0.30m,
            MonthlyCost = 0m,
            YearlyCost = 0m,
            OneTimeCost = 0m,
            IsAccepted = true,
        };

        var bank = CreateBankOffer(2.80m, bonus);

        var result = BonusEvaluator.Evaluate(bank, 200_000m, 25, 3_500m, 0m);

        var eval = result.BonusEvaluations.Single();
        eval.IsWorthIt.Should().BeTrue();
        eval.NetGainOrLoss.Should().BeGreaterThan(0);
        eval.InterestSavings.Should().BeGreaterThan(0);
        eval.TotalBonusCost.Should().Be(0m);
    }

    [Fact]
    public void Evaluate_ExpensiveBonusThatDoesNotCompensate_IsNotWorthIt()
    {
        var bonus = new Bonus
        {
            Name = "Alarm system",
            Category = BonusCategory.AlarmPackage,
            TinReductionPercentage = 0.05m,
            MonthlyCost = 45m,
            YearlyCost = 0m,
            OneTimeCost = 200m,
            IsAccepted = true,
        };

        var bank = CreateBankOffer(2.50m, bonus);

        var result = BonusEvaluator.Evaluate(bank, 200_000m, 25, 3_500m, 0m);

        var eval = result.BonusEvaluations.Single();
        // Monthly cost 45 * 12 * 25 = 13500 + 200 = 13700
        // TIN reduction of 0.05% on 200k for 25y saves much less
        eval.IsWorthIt.Should().BeFalse();
        eval.NetGainOrLoss.Should().BeNegative();
    }

    [Fact]
    public void Evaluate_FinalTin_ReflectsAcceptedBonuses()
    {
        var bonus1 = new Bonus
        {
            Name = "Payroll",
            TinReductionPercentage = 0.20m,
            IsAccepted = true,
        };
        var bonus2 = new Bonus
        {
            Name = "Home Insurance",
            TinReductionPercentage = 0.15m,
            IsAccepted = false,
        };
        var bonus3 = new Bonus
        {
            Name = "Life Insurance",
            TinReductionPercentage = 0.10m,
            IsMandatory = true,
            IsAccepted = false, // mandatory overrides
        };

        var bank = CreateBankOffer(3.00m, bonus1, bonus2, bonus3);

        var result = BonusEvaluator.Evaluate(bank, 200_000m, 25, 3_500m, 0m);

        // Accepted: Payroll (0.20) + Life (mandatory, 0.10) = 0.30
        result.FinalRealTin.Should().Be(2.70m);
        // Max theoretical: all = 0.20 + 0.15 + 0.10 = 0.45
        result.MaxTheoreticalDiscountedTin.Should().Be(2.55m);
    }

    [Fact]
    public void Evaluate_BonusWithLimitedDuration_CostCalculatedCorrectly()
    {
        var bonus = new Bonus
        {
            Name = "Card usage",
            Category = BonusCategory.Card,
            TinReductionPercentage = 0.10m,
            MonthlyCost = 0m,
            YearlyCost = 40m,
            OneTimeCost = 0m,
            DurationYears = 5,
            IsAccepted = true,
        };

        var bank = CreateBankOffer(2.50m, bonus);
        var result = BonusEvaluator.Evaluate(bank, 200_000m, 25, 3_500m, 0m);

        var eval = result.BonusEvaluations.Single();
        // Cost = 40 * 5 = 200 (only for 5 years, not 25)
        eval.TotalBonusCost.Should().Be(200m);
    }

    [Fact]
    public void Evaluate_MultipleBanks_AssignCorrectRecommendations()
    {
        var bank1 = BonusEvaluator.Evaluate(
            new BankOffer { BankName = "Good Bank", BaseTinPercentage = 2.30m, MortgageTermsYears = [25] },
            200_000m, 25, 3_500m, 0m);

        var bank2 = BonusEvaluator.Evaluate(
            new BankOffer { BankName = "OK Bank", BaseTinPercentage = 2.50m, MortgageTermsYears = [25] },
            200_000m, 25, 3_500m, 0m);

        var bank3 = BonusEvaluator.Evaluate(
            new BankOffer { BankName = "Bad Bank", BaseTinPercentage = 3.50m, MortgageTermsYears = [25] },
            200_000m, 25, 3_500m, 0m);

        var banks = new List<BankOfferResult> { bank1, bank2, bank3 };
        BonusEvaluator.AssignRecommendations(banks, 35m, 25);

        bank1.Recommendation.Should().Be(BankRecommendationLevel.Recommended);
        bank3.Recommendation.Should().Be(BankRecommendationLevel.NotWorthIt);
    }

    [Fact]
    public void Evaluate_RealGlobalCost_IncludesPrincipalInterestAndBonusCosts()
    {
        var bonus = new Bonus
        {
            Name = "Home Insurance",
            TinReductionPercentage = 0.20m,
            YearlyCost = 300m,
            IsAccepted = true,
        };

        var bank = CreateBankOffer(2.50m, bonus);
        var result = BonusEvaluator.Evaluate(bank, 200_000m, 25, 3_500m, 0m);

        // Global cost = principal + interest at final TIN + bonus costs
        // Bonus cost = 300 * 25 = 7500
        result.TotalAcceptedBonusCost.Should().Be(7_500m);
        result.RealGlobalCost.Should().BeGreaterThan(200_000m + 7_500m);
    }
}
