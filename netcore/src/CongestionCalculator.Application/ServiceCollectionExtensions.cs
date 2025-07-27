using Microsoft.Extensions.DependencyInjection;

namespace CongestionCalculator.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TaxCalculatorService>();

        return services;
    }
}
