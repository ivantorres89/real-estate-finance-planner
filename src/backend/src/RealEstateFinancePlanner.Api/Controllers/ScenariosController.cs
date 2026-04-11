using Microsoft.AspNetCore.Mvc;
using RealEstateFinancePlanner.Api.Dtos;
using RealEstateFinancePlanner.Api.Mapping;
using RealEstateFinancePlanner.Application.Services;

namespace RealEstateFinancePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScenariosController : ControllerBase
{
    private readonly ScenarioService _scenarioService;

    public ScenariosController(ScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ScenarioListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var scenarios = await _scenarioService.GetAllAsync(ct);
        return Ok(scenarios.Select(s => s.ToListItem()).ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ScenarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var scenario = await _scenarioService.GetByIdAsync(id, ct);
        if (scenario == null) return NotFound();
        return Ok(scenario.ToResponse());
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScenarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateScenarioRequest request, CancellationToken ct)
    {
        var entity = request.ToEntity();
        var created = await _scenarioService.CreateAsync(entity, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToResponse());
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ScenarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateScenarioRequest request, CancellationToken ct)
    {
        var existing = await _scenarioService.GetByIdAsync(id, ct);
        if (existing == null) return NotFound();

        var entity = request.ToEntity();
        entity.Id = id;
        entity.CreatedAt = existing.CreatedAt;

        var updated = await _scenarioService.UpdateAsync(entity, ct);
        return Ok(updated.ToResponse());
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted = await _scenarioService.DeleteAsync(id, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/analyze")]
    [ProducesResponseType(typeof(AnalysisResultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Analyze(string id, CancellationToken ct)
    {
        try
        {
            var result = await _scenarioService.RunAnalysisAsync(id, ct);
            return Ok(result.ToDto());
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
