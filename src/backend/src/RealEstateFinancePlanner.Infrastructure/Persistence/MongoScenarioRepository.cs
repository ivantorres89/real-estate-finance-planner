using MongoDB.Bson;
using MongoDB.Driver;
using RealEstateFinancePlanner.Application.Interfaces;
using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Infrastructure.Persistence;

public class MongoScenarioRepository : IScenarioRepository
{
    private readonly MongoDbContext _context;

    public MongoScenarioRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Scenario?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var filter = Builders<Scenario>.Filter.Eq(s => s.Id, id);
        var scenario = await _context.Scenarios.Find(filter).FirstOrDefaultAsync(ct);
        return MigrateLegacy(scenario);
    }

    public async Task<List<Scenario>> GetAllAsync(CancellationToken ct = default)
    {
        var scenarios = await _context.Scenarios
            .Find(Builders<Scenario>.Filter.Empty)
            .SortByDescending(s => s.UpdatedAt)
            .ToListAsync(ct);

        foreach (var s in scenarios)
        {
            MigrateLegacy(s);
        }
        return scenarios;
    }

    public async Task<Scenario> CreateAsync(Scenario scenario, CancellationToken ct = default)
    {
        scenario.Id = string.Empty; // Let MongoDB generate the ID
        await _context.Scenarios.InsertOneAsync(scenario, cancellationToken: ct);
        return scenario;
    }

    public async Task<Scenario> UpdateAsync(Scenario scenario, CancellationToken ct = default)
    {
        var filter = Builders<Scenario>.Filter.Eq(s => s.Id, scenario.Id);
        var result = await _context.Scenarios.ReplaceOneAsync(filter, scenario, cancellationToken: ct);

        if (result.MatchedCount == 0)
            throw new KeyNotFoundException($"Scenario '{scenario.Id}' not found.");

        return scenario;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var filter = Builders<Scenario>.Filter.Eq(s => s.Id, id);
        var result = await _context.Scenarios.DeleteOneAsync(filter, ct);
        return result.DeletedCount > 0;
    }

    /// <summary>
    /// One-off migration on read: older scenarios stored a single SalePrice / PurchasePrice /
    /// DeedPrice. Map them into the new A/B model: A absorbs the whole declared amount,
    /// B is 0. The previous DeedPrice becomes AppraisalValue. The cached LastResult is
    /// cleared because the result schema changed; the user must re-run the analysis.
    /// </summary>
    private static Scenario? MigrateLegacy(Scenario? scenario)
    {
        if (scenario == null) return null;

        bool migrated = false;

        if (scenario.Sale.LegacyExtraElements is { Count: > 0 } saleLegacy)
        {
            if (scenario.Sale.OfficialSalePriceA == 0m
                && TryReadDecimal(saleLegacy, "SalePrice", out var legacySalePrice)
                && legacySalePrice > 0m)
            {
                scenario.Sale.OfficialSalePriceA = legacySalePrice;
                scenario.Sale.UnofficialSalePriceB = 0m;
                migrated = true;
            }
            scenario.Sale.LegacyExtraElements = null;
        }

        if (scenario.Purchase.LegacyExtraElements is { Count: > 0 } purchaseLegacy)
        {
            bool hadPurchasePrice = TryReadDecimal(purchaseLegacy, "PurchasePrice", out var legacyPurchasePrice);
            bool hadDeedPrice = TryReadDecimal(purchaseLegacy, "DeedPrice", out var legacyDeedPrice);

            if (scenario.Purchase.OfficialPurchasePriceA == 0m && hadPurchasePrice && legacyPurchasePrice > 0m)
            {
                scenario.Purchase.OfficialPurchasePriceA = legacyPurchasePrice;
                scenario.Purchase.UnofficialPurchasePriceB = 0m;
                migrated = true;
            }
            if (scenario.Purchase.AppraisalValue == 0m && hadDeedPrice && legacyDeedPrice > 0m)
            {
                scenario.Purchase.AppraisalValue = legacyDeedPrice;
                migrated = true;
            }
            scenario.Purchase.LegacyExtraElements = null;
        }

        if (migrated)
        {
            // Result schema changed too; drop the stale cached analysis to force a refresh.
            scenario.LastResult = null;
        }

        return scenario;
    }

    private static bool TryReadDecimal(IReadOnlyDictionary<string, object> bag, string key, out decimal value)
    {
        if (bag.TryGetValue(key, out var raw))
        {
            switch (raw)
            {
                case decimal d:
                    value = d;
                    return true;
                case Decimal128 d128:
                    value = Decimal128.ToDecimal(d128);
                    return true;
                case BsonValue bv when bv.IsDecimal128:
                    value = (decimal)bv.AsDecimal128;
                    return true;
                case BsonValue bv when bv.IsDouble:
                    value = (decimal)bv.AsDouble;
                    return true;
                case BsonValue bv when bv.IsInt32:
                    value = bv.AsInt32;
                    return true;
                case BsonValue bv when bv.IsInt64:
                    value = bv.AsInt64;
                    return true;
                case double dbl:
                    value = (decimal)dbl;
                    return true;
                case long l:
                    value = l;
                    return true;
                case int i:
                    value = i;
                    return true;
            }
        }
        value = 0m;
        return false;
    }
}
