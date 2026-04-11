using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Enums;
using RealEstateFinancePlanner.Infrastructure.Configuration;
using RealEstateFinancePlanner.Infrastructure.Persistence;
using Testcontainers.MongoDb;

namespace RealEstateFinancePlanner.Tests.Integration;

public class MongoScenarioRepositoryTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder("mongo:7")
        .Build();

    private MongoScenarioRepository _repository = null!;
    private MongoDbContext _context = null!;

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();

        _context = new MongoDbContext(
            _mongoContainer.GetConnectionString(),
            "test_real_estate_planner");

        _repository = new MongoScenarioRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }

    private static Scenario CreateTestScenario(string name = "Integration Test Scenario") => new()
    {
        Name = name,
        Sale = new SaleData
        {
            SalePrice = 250_000m,
            SaleRelatedCosts = 5_000m,
            OutstandingMortgageDebt = 120_000m,
            CurrentCashBalance = 30_000m,
        },
        Purchase = new PurchaseData
        {
            PurchasePrice = 300_000m,
            DeedPrice = 280_000m,
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
            MaxDebtRatioPercentage = 35m,
        },
        Banks =
        [
            new BankOffer
            {
                BankName = "Test Bank",
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
            AnalysisHorizonYears = 20,
            MinimumLiquidityCushion = 10_000m,
            RiskProfile = RiskProfile.Balanced,
        }
    };

    [Fact]
    public async Task CreateAndRetrieve_Scenario_RoundTripsCorrectly()
    {
        var scenario = CreateTestScenario();

        var created = await _repository.CreateAsync(scenario);

        created.Id.Should().NotBeNullOrEmpty();

        var retrieved = await _repository.GetByIdAsync(created.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Integration Test Scenario");
        retrieved.Sale.SalePrice.Should().Be(250_000m);
        retrieved.Purchase.DeedPrice.Should().Be(280_000m);
        retrieved.DebtCapacity.MonthlyNetSalary.Should().Be(3_500m);
        retrieved.Banks.Should().HaveCount(1);
        retrieved.Banks[0].BankName.Should().Be("Test Bank");
        retrieved.Banks[0].Bonuses.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_ReturnsAllScenarios()
    {
        await _repository.CreateAsync(CreateTestScenario("Scenario A"));
        await _repository.CreateAsync(CreateTestScenario("Scenario B"));

        var all = await _repository.GetAllAsync();

        all.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Update_Scenario_PersistsChanges()
    {
        var scenario = CreateTestScenario();
        var created = await _repository.CreateAsync(scenario);

        created.Name = "Updated Name";
        created.Sale.SalePrice = 300_000m;

        await _repository.UpdateAsync(created);

        var retrieved = await _repository.GetByIdAsync(created.Id);
        retrieved!.Name.Should().Be("Updated Name");
        retrieved.Sale.SalePrice.Should().Be(300_000m);
    }

    [Fact]
    public async Task Delete_Scenario_RemovesIt()
    {
        var scenario = CreateTestScenario();
        var created = await _repository.CreateAsync(scenario);

        var deleted = await _repository.DeleteAsync(created.Id);
        deleted.Should().BeTrue();

        var retrieved = await _repository.GetByIdAsync(created.Id);
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task Delete_NonExistent_ReturnsFalse()
    {
        var deleted = await _repository.DeleteAsync("000000000000000000000000");
        deleted.Should().BeFalse();
    }

    [Fact]
    public async Task DecimalPrecision_PreservedInMongoDB()
    {
        var scenario = CreateTestScenario();
        scenario.Sale.SalePrice = 123_456.78m;
        scenario.Purchase.FinanceablePercentage = 87.5m;

        var created = await _repository.CreateAsync(scenario);
        var retrieved = await _repository.GetByIdAsync(created.Id);

        retrieved!.Sale.SalePrice.Should().Be(123_456.78m);
        retrieved.Purchase.FinanceablePercentage.Should().Be(87.5m);
    }

    [Fact]
    public async Task AnalysisResult_PersistedWithScenario()
    {
        var scenario = CreateTestScenario();
        scenario.LastResult = new AnalysisResult
        {
            SaleLiquidity = new SaleLiquidityResult
            {
                NetSaleLiquidity = 125_000m,
                RealAvailableCash = 155_000m,
            },
            PurchaseCosts = new PurchaseCostResult
            {
                ItpAmount = 8_400m,
                MaxMortgageAmount = 224_000m,
                EntryPayment = 76_000m,
                TotalCashNeeded = 87_100m,
                RemainingLiquidity = 67_900m,
                IsViable = true,
            },
            DebtCapacity = new DebtCapacityResult
            {
                MaxMonthlyPaymentCapacity = 1_225m,
            },
        };

        var created = await _repository.CreateAsync(scenario);
        var retrieved = await _repository.GetByIdAsync(created.Id);

        retrieved!.LastResult.Should().NotBeNull();
        retrieved.LastResult!.SaleLiquidity.NetSaleLiquidity.Should().Be(125_000m);
        retrieved.LastResult.PurchaseCosts.IsViable.Should().BeTrue();
    }
}
