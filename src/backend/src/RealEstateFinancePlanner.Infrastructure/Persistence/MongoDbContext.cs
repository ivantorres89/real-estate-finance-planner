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
                      .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(BsonType.ObjectId))
                      .SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance);
                });
            }

            _classMapsRegistered = true;
        }
    }
}
