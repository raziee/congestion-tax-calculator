using CongestionCalculator.Application;
using CongestionCalculator.Application.Interfaces;
using CongestionCalculator.Domain.Enums;
using CongestionCalculator.Infrastructure.Data;
using CongestionCalculator.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CongestionCalculator.Tests;

public class CongestionTaxCalculatorTests
{
    private readonly TaxCalculatorService _calculator;

    public CongestionTaxCalculatorTests()
    {
        var serviceProvider = new ServiceCollection();

        serviceProvider.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDbForTesting");
        });
        serviceProvider.AddScoped<DatabaseInitializer>();
        serviceProvider.AddScoped<ICityRepository, CityRepository>();
        serviceProvider.AddScoped<TaxCalculatorService>();

        var services = serviceProvider.BuildServiceProvider();
        _calculator = services.GetRequiredService<TaxCalculatorService>();

        var databaseInitializer = services.GetRequiredService<DatabaseInitializer>();
        databaseInitializer.Initialize();
    }

    [Fact]
    public void CalculateTax_ExemptVehicle_ReturnsZero()
    {
        // Arrange
        var passages = new[] { new DateTime(2013, 2, 8, 7, 30, 0) }; // Should be taxed 0 SEK

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Motorcycle, passages, "Gothenburg");

        // Assert
        Assert.Equal(0, tax);
    }

    [Fact]
    public void CalculateTax_Weekend_ReturnsZero()
    {
        // Arrange (Saturday)
        var passages = new[]
        {
            new DateTime(2013, 2, 9, 7, 30, 0), // Saturday
        };

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(0, tax);
    }

    [Fact]
    public void CalculateTax_July_ReturnsZero()
    {
        // Arrange
        var passages = new[]
        {
            new DateTime(2013, 7, 15, 7, 30, 0), // July
        };

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(0, tax);
    }

    [Fact]
    public void CalculateTax_PublicHoliday_ReturnsZero()
    {
        // Arrange (New Year's Day)
        var passages = new[] { new DateTime(2013, 1, 1, 7, 30, 0) };

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(0, tax);
    }

    [Fact]
    public void CalculateTax_DayBeforeHoliday_ReturnsZero()
    {
        // Arrange (Day before May Day)
        var passages = new[] { new DateTime(2013, 4, 30, 7, 30, 0) };

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(0, tax);
    }

    [Fact]
    public void CalculateTax_SinglePassage_CorrectTax()
    {
        // Arrange
        var passages = new[]
        {
            new DateTime(2013, 2, 7, 6, 23, 27), // Should be taxed 8 SEK
        };

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(8, tax);
    }

    [Fact]
    public void CalculateTax_MultiplePassagesSameDay_SingleChargeRuleApplied()
    {
        // Arrange
        var passages = new[]
        {
            new DateTime(2013, 2, 8, 6, 20, 27), // 8 SEK
            new DateTime(2013, 2, 8, 6, 37, 0), // 13 SEK (within 60 min, max is 13)
        };

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");
        
        // Assert
        Assert.Equal(13, tax);
    }

    [Fact]
    public void CalculateTax_MultiplePassagesSameDay_DifferentGroups()
    {
        // Arrange
        var passages = new[]
        {
            new DateTime(2013, 2, 8, 6, 20, 27), // Group 1: 8 SEK
            new DateTime(2013, 2, 8, 7, 15, 0), // Group 2: 18 SEK
            new DateTime(2013, 2, 8, 8, 30, 0), // Group 2: 8 SEK
        };

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(8 + 18, tax);
    }

    [Fact]
    public void CalculateTax_MaxDailyTax_Returns60()
    {
        // Arrange
        var passages = new[]
        {
            new DateTime(2013, 2, 8, 7, 0, 0), // 18 SEK
            new DateTime(2013, 2, 8, 7, 30, 0), // Same group (max 18)
            new DateTime(2013, 2, 8, 8, 0, 0), // 13 SEK
            new DateTime(2013, 2, 8, 8, 15, 0), // Same group (max 13)
            new DateTime(2013, 2, 8, 15, 0, 0), // 13 SEK
            new DateTime(2013, 2, 8, 15, 15, 0), // Same group (max 13)
            new DateTime(2013, 2, 8, 16, 0, 0), // 18 SEK
            new DateTime(2013, 2, 8, 16, 30, 0), // Same group (max 18)
            new DateTime(2013, 2, 8, 17, 0, 0), // 13 SEK
            new DateTime(2013, 2, 8, 17, 30, 0), // Same group (max 13)
        };

        // Total without cap: 18 + 13 + 13 + 18 + 13 = 75 SEK
        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(60, tax);
    }

    [Fact]
    public void CalculateTax_MultipleDays_CorrectSum()
    {
        // Arrange
        var passages = new[]
        {
            // Day 1: 8 + 13 = 21
            new DateTime(2013, 2, 7, 6, 23, 27), // 8 SEK
            new DateTime(2013, 2, 7, 15, 27, 0), // 13 SEK
            // Day 2: 8 + 8 + 13 + 18 + 13 = 57
            new DateTime(2013, 2, 8, 6, 27, 0), // 8 SEK (group 1)
            new DateTime(2013, 2, 8, 14, 35, 0), // 8 SEK  (group 2)
            new DateTime(2013, 2, 8, 15, 29, 0), // 13 SEK (group 3)
            new DateTime(2013, 2, 8, 15, 47, 0), // 18 SEK (group 3)
            new DateTime(2013, 2, 8, 17, 49, 0), // 13 SEK (group 4)
            // Day 3: 0 (Sunday)
            new DateTime(2013, 2, 10, 12, 0, 0),
            // Day 4: 8 SEK
            new DateTime(2013, 3, 26, 14, 25, 0),
        };

        // Expected total: 21 + 52 + 0 + 8 = 86
        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(81, tax);
    }

    [Fact]
    public void CalculateTax_EdgeCaseTimes_CorrectTax()
    {
        // Arrange
        var passages = new[]
        {
            new DateTime(2013, 2, 8, 6, 0, 0), // 8 SEK
            new DateTime(2013, 2, 8, 6, 29, 59), // 8 SEK (same group)
            new DateTime(2013, 2, 8, 6, 30, 0), // 13 SEK (same group)
            new DateTime(2013, 2, 8, 6, 59, 59), // 13 SEK (same group)
            new DateTime(2013, 2, 8, 18, 29, 59), // 8 SEK (new group)
            new DateTime(2013, 2, 8, 18, 30, 0), // 0 SEK
        };

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Groups: [8,8,13,13] max=13, [8] max=8, [0] max=0
        // Assert
        Assert.Equal(13 + 8, tax);
    }

    [Fact]
    public void CalculateTax_AfterMidnight_CorrectGrouping()
    {
        // Arrange
        var passages = new[]
        {
            new DateTime(2013, 2, 8, 23, 30, 0), // 0 SEK
            new DateTime(2013, 2, 9, 0, 15, 0), // 0 SEK (Saturday)
            new DateTime(2013, 2, 11, 5, 59, 59), // 0 SEK
            new DateTime(2013, 2, 11, 6, 0, 0), // 8 SEK (Monday)
        };

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(8, tax);
    }

    [Fact]
    public void CalculateTax_ComplexScenario_Feb8_CorrectTax()
    {
        // Arrange (2013-02-08 passages from the example)
        var passages = new[]
        {
            new DateTime(2013, 02, 08, 06, 27, 00), // Group 1: 13 SEK (06:27)
            new DateTime(2013, 02, 08, 06, 20, 27), // Group 1: 8 SEK -> max 13
            new DateTime(2013, 02, 08, 14, 35, 00), // Group 2: 8 SEK
            new DateTime(2013, 02, 08, 15, 29, 00), // Group 3: 13 SEK (15:29)
            new DateTime(2013, 02, 08, 15, 47, 00), // Group 3: 18 SEK (15:47) -> max 18
            new DateTime(2013, 02, 08, 16, 01, 00), // Group 3: 18 SEK -> max 18
            new DateTime(2013, 02, 08, 16, 48, 00), // Group 4: 18 SEK (16:48)
            new DateTime(2013, 02, 08, 17, 49, 00), // Group 5: 13 SEK (17:49)
            new DateTime(2013, 02, 08, 18, 29, 00), // Group 6: 8 SEK (18:29)
            new DateTime(2013, 02, 08, 18, 35, 00), // Group 7: 0 SEK
        };

        // Groups:
        // 1: [06:20, 06:27] -> max 13
        // 2: [14:35] -> 8
        // 3: [15:29, 15:47, 16:01] -> max 18
        // 4: [16:48] -> 18
        // 5: [17:49] -> 13
        // 6: [18:29] -> 8
        // 7: [18:35] -> 0
        // Total: 13 + 8 + 18 + 18 + 13 + 8 = 78 -> capped at 60

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(60, tax);
    }

    [Fact]
    public void CalculateTax_FullScenario_CorrectTotal()
    {
        // Arrange (all passages from the example)
        var passages = new[]
        {
            new DateTime(2013, 01, 14, 21, 00, 00), // 0 (after hours)
            new DateTime(2013, 01, 15, 21, 00, 00), // 0 (after hours)
            new DateTime(2013, 02, 07, 06, 23, 27), // 8 (morning)
            new DateTime(2013, 02, 07, 15, 27, 00), // 13 (afternoon)
            new DateTime(2013, 02, 08, 06, 27, 00), // 13 (group 1)
            new DateTime(2013, 02, 08, 06, 20, 27), // 8 (group 1) -> max 13
            new DateTime(2013, 02, 08, 14, 35, 00), // 8 (group 2)
            new DateTime(2013, 02, 08, 15, 29, 00), // 13 (group 3)
            new DateTime(2013, 02, 08, 15, 47, 00), // 18 (group 3) -> max 18
            new DateTime(2013, 02, 08, 16, 01, 00), // 18 (group 3) -> max 18
            new DateTime(2013, 02, 08, 16, 48, 00), // 18 (group 4)
            new DateTime(2013, 02, 08, 17, 49, 00), // 13 (group 5)
            new DateTime(2013, 02, 08, 18, 29, 00), // 8 (group 6)
            new DateTime(2013, 02, 08, 18, 35, 00), // 0 (group 7)
            new DateTime(2013, 03, 26, 14, 25, 00), // 8
            new DateTime(2013, 03, 28, 14, 07, 27), // 0 (public holiday)
        };

        // Breakdown:
        // 2013-01-14: 0
        // 2013-01-15: 0
        // 2013-02-07: 8 + 13 = 21
        // 2013-02-08: 13 (group1) + 8 (group2) + 18 (group3) + 18 (group4) + 13 (group5) + 8 (group6) = 78 -> capped at 60
        // 2013-03-26: 8
        // 2013-03-28: 0
        // Total: 21 + 60 + 8 = 89

        // Act
        var tax = _calculator.CalculateTax(VehicleType.Car, passages, "Gothenburg");

        // Assert
        Assert.Equal(89, tax);
    }
}
