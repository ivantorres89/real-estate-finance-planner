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
        return await _context.Scenarios.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<List<Scenario>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Scenarios
            .Find(Builders<Scenario>.Filter.Empty)
            .SortByDescending(s => s.UpdatedAt)
            .ToListAsync(ct);
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
}
