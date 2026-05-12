using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using RealEstateFinancePlanner.Domain.Entities;
using RealEstateFinancePlanner.Infrastructure.Configuration;

namespace RealEstateFinancePlanner.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        RegisterClassMaps();

        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public MongoDbContext(string connectionString, string databaseName)
    {
        RegisterClassMaps();

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Scenario> Scenarios
        => _database.GetCollection<Scenario>("scenarios");

    private static bool _classMapsRegistered;
    private static readonly object _lock = new();

    private static void RegisterClassMaps()
    {
        lock (_lock)
        {
            if (_classMapsRegistered) return;

            // Store decimals as Decimal128 in MongoDB for full precision
            BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));

            if (!BsonClassMap.IsClassMapRegistered(typeof(Scenario)))
            {
                BsonClassMap.RegisterClassMap<Scenario>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(s => s.Id)
                      .SetSerializer(new StringSerializer(BsonType.ObjectId))
                      .SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(SaleData)))
            {
                BsonClassMap.RegisterClassMap<SaleData>(cm =>
                {
                    cm.AutoMap();
                    cm.MapExtraElementsProperty(nameof(SaleData.LegacyExtraElements));
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(PurchaseData)))
            {
                BsonClassMap.RegisterClassMap<PurchaseData>(cm =>
                {
                    cm.AutoMap();
                    cm.MapExtraElementsProperty(nameof(PurchaseData.LegacyExtraElements));
                });
            }

            // AnalysisResult, SaleLiquidityResult, PurchaseCostResult: schema changed.
            // Old documents may still contain legacy fields under LastResult.* We tell
            // the driver to ignore extra elements on results to avoid crashing on
            // legacy serialised payloads. The user must re-run the analysis to refresh
            // them with the new A/B model.
            if (!BsonClassMap.IsClassMapRegistered(typeof(SaleLiquidityResult)))
            {
                BsonClassMap.RegisterClassMap<SaleLiquidityResult>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(PurchaseCostResult)))
            {
                BsonClassMap.RegisterClassMap<PurchaseCostResult>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            _classMapsRegistered = true;
        }
    }
}
