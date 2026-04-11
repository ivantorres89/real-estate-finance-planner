using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateFinancePlanner.Application.Interfaces;
using RealEstateFinancePlanner.Application.Services;
using RealEstateFinancePlanner.Infrastructure.Configuration;
using RealEstateFinancePlanner.Infrastructure.Persistence;

namespace RealEstateFinancePlanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IScenarioRepository, MongoScenarioRepository>();
        services.AddScoped<IAnalysisService, AnalysisService>();
        services.AddScoped<ScenarioService>();

        return services;
    }
}
