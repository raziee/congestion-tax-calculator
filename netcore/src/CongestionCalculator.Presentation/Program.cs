using CongestionCalculator.Application;
using CongestionCalculator.Domain.Entities;
using CongestionCalculator.Domain.Enums;
using CongestionCalculator.Infrastructure;
using CongestionCalculator.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .AddInfrastructure()
    .AddApplication()
    .BuildServiceProvider();

var databaseInitializer = serviceProvider.GetRequiredService<DatabaseInitializer>();
databaseInitializer.Initialize();

var taxCalculatorService = serviceProvider.GetRequiredService<TaxCalculatorService>();

var passages = new[]
{
    new DateTime(2013, 01, 14, 21, 00, 00),
    new DateTime(2013, 01, 15, 21, 00, 00),
    new DateTime(2013, 02, 07, 06, 23, 27),
    new DateTime(2013, 02, 07, 15, 27, 00),
    new DateTime(2013, 02, 08, 06, 27, 00),
    new DateTime(2013, 02, 08, 06, 20, 27),
    new DateTime(2013, 02, 08, 14, 35, 00),
    new DateTime(2013, 02, 08, 15, 29, 00),
    new DateTime(2013, 02, 08, 15, 47, 00),
    new DateTime(2013, 02, 08, 16, 01, 00),
    new DateTime(2013, 02, 08, 16, 48, 00),
    new DateTime(2013, 02, 08, 17, 49, 00),
    new DateTime(2013, 02, 08, 18, 29, 00),
    new DateTime(2013, 02, 08, 18, 35, 00),
    new DateTime(2013, 03, 26, 14, 25, 00),
    new DateTime(2013, 03, 28, 14, 07, 27),
};

var tax = taxCalculatorService.CalculateTax(VehicleType.Car, passages, "Gothenburg");
Console.WriteLine($"Total Tax: {tax} SEK");
