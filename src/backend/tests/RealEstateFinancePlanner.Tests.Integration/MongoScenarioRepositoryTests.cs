using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Domain.Enums;
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
        retrieved.Sale.OfficialSalePriceA.Should().Be(250_000m);
        retrieved.Sale.UnofficialSalePriceB.Should().Be(0m);
        retrieved.Purchase.OfficialPurchasePriceA.Should().Be(280_000m);
        retrieved.Purchase.UnofficialPurchasePriceB.Should().Be(20_000m);
        retrieved.Purchase.AppraisalValue.Should().Be(280_000m);
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
        created.Sale.OfficialSalePriceA = 300_000m;
        created.Sale.UnofficialSalePriceB = 25_000m;

        await _repository.UpdateAsync(created);

        var retrieved = await _repository.GetByIdAsync(created.Id);
        retrieved!.Name.Should().Be("Updated Name");
        retrieved.Sale.OfficialSalePriceA.Should().Be(300_000m);
        retrieved.Sale.UnofficialSalePriceB.Should().Be(25_000m);
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
        scenario.Sale.OfficialSalePriceA = 123_456.78m;
        scenario.Purchase.FinanceablePercentage = 87.5m;

        var created = await _repository.CreateAsync(scenario);
        var retrieved = await _repository.GetByIdAsync(created.Id);

        retrieved!.Sale.OfficialSalePriceA.Should().Be(123_456.78m);
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
                OfficialSalePriceA = 250_000m,
                UnofficialSalePriceB = 0m,
                TotalSalePrice = 250_000m,
                NetSaleLiquidityA = 125_000m,
                NetSaleLiquidityB = 0m,
                RealAvailableCashA = 155_000m,
                RealAvailableCashB = 0m,
                TotalRealAvailableCash = 155_000m,
                IsSaleViable = true,
            },
            PurchaseCosts = new PurchaseCostResult
            {
                OfficialPurchasePriceA = 280_000m,
                UnofficialPurchasePriceB = 20_000m,
                TotalPurchasePrice = 300_000m,
                ItpAmount = 8_400m,
                MaxMortgageAmount = 224_000m,
                EntryPaymentA = 56_000m,
                EntryPaymentB = 20_000m,
                TotalCashNeededA = 67_100m,
                TotalCashNeededB = 20_000m,
                RemainingLiquidityA = 87_900m,
                RemainingLiquidityB = -20_000m,
                IsViable = false,
                BalancingAdvice = "• La parte B no cubre…",
            },
            DebtCapacity = new DebtCapacityResult
            {
                MaxMonthlyPaymentCapacity = 1_225m,
            },
        };

        var created = await _repository.CreateAsync(scenario);
        var retrieved = await _repository.GetByIdAsync(created.Id);

        retrieved!.LastResult.Should().NotBeNull();
        retrieved.LastResult!.SaleLiquidity.NetSaleLiquidityA.Should().Be(125_000m);
        retrieved.LastResult.PurchaseCosts.IsViable.Should().BeFalse();
        retrieved.LastResult.PurchaseCosts.EntryPaymentA.Should().Be(56_000m);
        retrieved.LastResult.PurchaseCosts.BalancingAdvice.Should().Contain("La parte B no cubre");
    }

    [Fact]
    public async Task LegacyDocument_WithOldSchema_IsMigratedOnRead()
    {
        // Simulate a pre-A/B-split document inserted directly into Mongo by older code.
        var legacy = new BsonDocument
        {
            { "Name", "Legacy Scenario" },
            {
                "Sale", new BsonDocument
                {
                    { "SalePrice", new BsonDecimal128(250_000m) },
                    { "SaleRelatedCosts", new BsonDecimal128(5_000m) },
                    { "OutstandingMortgageDebt", new BsonDecimal128(120_000m) },
                    { "CurrentCashBalance", new BsonDecimal128(30_000m) },
                    { "MunicipalCapitalGainsTax", new BsonDecimal128(0m) },
                    { "ExtraordinaryCosts", new BsonDecimal128(0m) },
                }
            },
            {
                "Purchase", new BsonDocument
                {
                    { "PurchasePrice", new BsonDecimal128(300_000m) },
                    { "DeedPrice", new BsonDecimal128(280_000m) },
                    { "FinanceablePercentage", new BsonDecimal128(80m) },
                    { "NotaryCosts", new BsonDecimal128(0m) },
                    { "AdministrativeCosts", new BsonDecimal128(0m) },
                    { "AppraisalCosts", new BsonDecimal128(0m) },
                    { "AgencyCosts", new BsonDecimal128(0m) },
                    { "OtherCosts", new BsonDecimal128(0m) },
                    { "ApplyReducedItp", true },
                    { "IsMainResidence", true },
                    { "BuyerAge", 30 },
                }
            },
            {
                "DebtCapacity", new BsonDocument
                {
                    { "MonthlyNetSalary", new BsonDecimal128(3_000m) },
                    { "MonthlyOutstandingLoanPayments", new BsonDecimal128(0m) },
                    { "MaxDebtRatioPercentage", new BsonDecimal128(35m) },
                }
            },
            { "Banks", new BsonArray() },
            { "StrategyParameters", new BsonDocument
                {
                    { "ExpectedAnnualReturnConservative", new BsonDecimal128(3m) },
                    { "ExpectedAnnualReturnBase", new BsonDecimal128(6m) },
                    { "ExpectedAnnualReturnOptimistic", new BsonDecimal128(9m) },
                    { "UseNetReturns", true },
                    { "AnalysisHorizonYears", 20 },
                    { "MinimumLiquidityCushion", new BsonDecimal128(10_000m) },
                    { "RiskProfile", "Balanced" },
                    { "AdditionalCapitalToPreserve", new BsonDecimal128(0m) },
                }
            },
            { "CreatedAt", DateTime.UtcNow },
            { "UpdatedAt", DateTime.UtcNow },
        };

        var rawCollection = _context.Scenarios.Database.GetCollection<BsonDocument>("scenarios");
        await rawCollection.InsertOneAsync(legacy);
        var id = legacy["_id"].AsObjectId.ToString();

        var migrated = await _repository.GetByIdAsync(id);

        migrated.Should().NotBeNull();
        migrated!.Sale.OfficialSalePriceA.Should().Be(250_000m);
        migrated.Sale.UnofficialSalePriceB.Should().Be(0m);
        migrated.Sale.LegacyExtraElements.Should().BeNull();
        migrated.Purchase.OfficialPurchasePriceA.Should().Be(300_000m);
        migrated.Purchase.UnofficialPurchasePriceB.Should().Be(0m);
        migrated.Purchase.AppraisalValue.Should().Be(280_000m);
        migrated.Purchase.RenovationCostsB.Should().Be(0m);
        migrated.Purchase.LegacyExtraElements.Should().BeNull();
        migrated.LastResult.Should().BeNull(); // Cleared because schema changed
    }
}
