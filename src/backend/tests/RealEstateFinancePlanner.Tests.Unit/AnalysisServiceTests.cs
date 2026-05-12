using FluentAssertions;
using RealEstateFinancePlanner.Application.Services;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Enums;

namespace RealEstateFinancePlanner.Tests.Unit;

public class AnalysisServiceTests
{
    private readonly AnalysisService _sut = new();

    [Fact]
    public void RunFullAnalysis_CompleteScenario_ProducesAllResults()
    {
        var scenario = new Scenario
        {
            Name = "Test Scenario",
            Sale = new SaleData
            {
                OfficialSalePriceA = 250_000m,
                UnofficialSalePriceB = 0m,
                SaleRelatedCosts = 5_000m,
                OutstandingMortgageDebt = 120_000m,
                CurrentCashBalance = 30_000m,
            },
            Purchase = new PurchaseData
            {
                OfficialPurchasePriceA = 280_000m,
                UnofficialPurchasePriceB = 20_000m,
                AppraisalValue = 280_000m,
                FinanceablePercentage = 80m,
                ApplyReducedItp = true,
                NotaryCosts = 1_500m,
                AdministrativeCosts = 800m,
                AppraisalCosts = 400m,
                IsMainResidence = true,
                BuyerAge = 35,
            },
            DebtCapacity = new DebtCapacityData
            {
                MonthlyNetSalary = 3_500m,
                MonthlyOutstandingLoanPayments = 0m,
                MaxDebtRatioPercentage = 35m,
            },
            Banks =
            [
                new BankOffer
                {
                    BankName = "Bank A",
                    BaseTinPercentage = 2.50m,
                    MortgageTermsYears = [20, 25, 30],
                    Bonuses =
                    [
                        new Bonus
                        {
                            Name = "Payroll",
                            Category = BonusCategory.Payroll,
                            TinReductionPercentage = 0.20m,
                            IsAccepted = true,
                        }
                    ]
                }
            ],
            StrategyParameters = new StrategyParameters
            {
                ExpectedAnnualReturnConservative = 3m,
                ExpectedAnnualReturnBase = 6m,
                ExpectedAnnualReturnOptimistic = 9m,
                AnalysisHorizonYears = 20,
                MinimumLiquidityCushion = 10_000m,
                RiskProfile = RiskProfile.Balanced,
            }
        };

        var result = _sut.RunFullAnalysis(scenario);

        result.Should().NotBeNull();
        result.SaleLiquidity.Should().NotBeNull();
        result.SaleLiquidity.NetSaleLiquidityA.Should().Be(125_000m);
        result.SaleLiquidity.RealAvailableCashA.Should().Be(155_000m);
        result.SaleLiquidity.NetSaleLiquidityB.Should().Be(0m);

        result.PurchaseCosts.Should().NotBeNull();
        result.PurchaseCosts.ItpAmount.Should().Be(8_400m);
        result.PurchaseCosts.MaxMortgageAmount.Should().Be(224_000m);
        result.PurchaseCosts.EntryPaymentA.Should().Be(56_000m);
        result.PurchaseCosts.EntryPaymentB.Should().Be(20_000m);
        result.PurchaseCosts.IsViable.Should().BeFalse(); // B side has no cash
        result.PurchaseCosts.RemainingLiquidityB.Should().Be(-20_000m);

        result.DebtCapacity.Should().NotBeNull();
        result.DebtCapacity.MaxMonthlyPaymentCapacity.Should().Be(1_225m);

        result.BankResults.Should().HaveCount(1);
        result.BankResults[0].BankName.Should().Be("Bank A");
        result.BankResults[0].FinalRealTin.Should().Be(2.30m);

        result.StrategyComparison.Should().NotBeNull();
        result.StrategyComparison!.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void RunFullAnalysis_NoBanks_NoStrategyComparison()
    {
        var scenario = new Scenario
        {
            Name = "No Banks",
            Sale = new SaleData
            {
                OfficialSalePriceA = 200_000m,
                CurrentCashBalance = 50_000m,
            },
            Purchase = new PurchaseData
            {
                OfficialPurchasePriceA = 240_000m,
                UnofficialPurchasePriceB = 10_000m,
                AppraisalValue = 240_000m,
                FinanceablePercentage = 80m,
                ApplyReducedItp = true,
                BuyerAge = 30,
            },
            DebtCapacity = new DebtCapacityData
            {
                MonthlyNetSalary = 3_000m,
                MaxDebtRatioPercentage = 35m,
            },
        };

        var result = _sut.RunFullAnalysis(scenario);

        result.BankResults.Should().BeEmpty();
        result.StrategyComparison.Should().BeNull();
    }

    [Fact]
    public void RunFullAnalysis_NullScenario_Throws()
    {
        var act = () => _sut.RunFullAnalysis(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
