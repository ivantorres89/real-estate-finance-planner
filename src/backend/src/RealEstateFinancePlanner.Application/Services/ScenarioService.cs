using RealEstateFinancePlanner.Application.Interfaces;
using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Application.Services;

public class ScenarioService
{
    private readonly IScenarioRepository _repository;
    private readonly IAnalysisService _analysisService;

    public ScenarioService(IScenarioRepository repository, IAnalysisService analysisService)
    {
        _repository = repository;
        _analysisService = analysisService;
    }

    public Task<List<Scenario>> GetAllAsync(CancellationToken ct = default)
        => _repository.GetAllAsync(ct);

    public Task<Scenario?> GetByIdAsync(string id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);

    public async Task<Scenario> CreateAsync(Scenario scenario, CancellationToken ct = default)
    {
        scenario.CreatedAt = DateTime.UtcNow;
        scenario.UpdatedAt = DateTime.UtcNow;
        return await _repository.CreateAsync(scenario, ct);
    }

    public async Task<Scenario> UpdateAsync(Scenario scenario, CancellationToken ct = default)
    {
        scenario.UpdatedAt = DateTime.UtcNow;
        return await _repository.UpdateAsync(scenario, ct);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken ct = default)
        => _repository.DeleteAsync(id, ct);

    public async Task<Scenario> DuplicateAsync(string sourceId, CancellationToken ct = default)
    {
        var source = await _repository.GetByIdAsync(sourceId, ct)
            ?? throw new KeyNotFoundException($"Scenario '{sourceId}' not found.");

        var clone = new Scenario
        {
            Name = $"Copy of {source.Name}",
            Sale = source.Sale,
            Purchase = source.Purchase,
            DebtCapacity = source.DebtCapacity,
            Banks = source.Banks,
            StrategyParameters = source.StrategyParameters,
            LastResult = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        return await _repository.CreateAsync(clone, ct);
    }

    public async Task<AnalysisResult> RunAnalysisAsync(string scenarioId, CancellationToken ct = default)
    {
        var scenario = await _repository.GetByIdAsync(scenarioId, ct)
            ?? throw new KeyNotFoundException($"Scenario '{scenarioId}' not found.");

        var result = _analysisService.RunFullAnalysis(scenario);

        scenario.LastResult = result;
        scenario.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(scenario, ct);

        return result;
    }
}
