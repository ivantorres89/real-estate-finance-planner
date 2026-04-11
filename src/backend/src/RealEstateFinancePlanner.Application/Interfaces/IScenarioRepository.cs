using RealEstateFinancePlanner.Domain.Entities;

namespace RealEstateFinancePlanner.Application.Interfaces;

public interface IScenarioRepository
{
    Task<Scenario?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<Scenario>> GetAllAsync(CancellationToken ct = default);
    Task<Scenario> CreateAsync(Scenario scenario, CancellationToken ct = default);
    Task<Scenario> UpdateAsync(Scenario scenario, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}
