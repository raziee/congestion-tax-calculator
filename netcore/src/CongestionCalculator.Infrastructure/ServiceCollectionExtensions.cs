using CongestionCalculator.Application.Interfaces;
using CongestionCalculator.Infrastructure.Data;
using CongestionCalculator.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CongestionCalculator.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=TaxDatabase.db")
        );
        services.AddScoped<DatabaseInitializer>();
        services.AddScoped<ICityRepository, CityRepository>();
        return services;
    }
}
