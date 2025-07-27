using CongestionCalculator.Domain.Entities;
using CongestionCalculator.Domain.Enums;

namespace CongestionCalculator.Infrastructure.Data;

public class DatabaseInitializer(AppDbContext _context)
{
    public void Initialize()
    {
        _context.Database.EnsureCreated();

        if (_context.Cities.Any())
            return;

        var gothenburg = new City(
            name: "Gothenburg",
            currency: "SEK",
            maxDailyTax: 60,
            singleChargeRule: true,
            singleChargeMinute: 60,
            noTaxBeforeHoliday: true
        );
        gothenburg.AddNonTaxableDayOfWeek(DayOfWeek.Saturday);
        gothenburg.AddNonTaxableDayOfWeek(DayOfWeek.Sunday);
        gothenburg.AddNonTaxableMonth(Month.July);
        gothenburg.AddExemptVehicle(VehicleType.Emergency);
        gothenburg.AddExemptVehicle(VehicleType.Bus);
        gothenburg.AddExemptVehicle(VehicleType.Diplomat);
        gothenburg.AddExemptVehicle(VehicleType.Motorcycle);
        gothenburg.AddExemptVehicle(VehicleType.Military);
        gothenburg.AddExemptVehicle(VehicleType.Foreign);
        gothenburg.AddTaxRule(new TimeSpan(6, 0, 0), new TimeSpan(6, 29, 59), 8);
        gothenburg.AddTaxRule(new TimeSpan(6, 30, 0), new TimeSpan(6, 59, 59), 13);
        gothenburg.AddTaxRule(new TimeSpan(7, 00, 0), new TimeSpan(7, 59, 59), 18);
        gothenburg.AddTaxRule(new TimeSpan(8, 00, 0), new TimeSpan(8, 29, 59), 13);
        gothenburg.AddTaxRule(new TimeSpan(8, 30, 0), new TimeSpan(14, 59, 59), 8);
        gothenburg.AddTaxRule(new TimeSpan(15, 00, 0), new TimeSpan(15, 29, 59), 13);
        gothenburg.AddTaxRule(new TimeSpan(15, 30, 0), new TimeSpan(16, 59, 59), 18);
        gothenburg.AddTaxRule(new TimeSpan(17, 00, 0), new TimeSpan(17, 59, 59), 13);
        gothenburg.AddTaxRule(new TimeSpan(18, 00, 0), new TimeSpan(18, 29, 59), 8);
        gothenburg.AddTaxRule(new TimeSpan(18, 30, 0), new TimeSpan(5, 59, 59), 0);
        gothenburg.AddHoliday(new DateTime(2013, 1, 1), "New Year's Day");
        gothenburg.AddHoliday(new DateTime(2013, 3, 28), "Maundy Thursday");
        gothenburg.AddHoliday(new DateTime(2013, 3, 29), "Good Friday");
        gothenburg.AddHoliday(new DateTime(2013, 4, 1), "Easter Monday");
        gothenburg.AddHoliday(new DateTime(2013, 5, 1), "May Day");
        gothenburg.AddHoliday(new DateTime(2013, 5, 9), "Ascension Day");
        gothenburg.AddHoliday(new DateTime(2013, 6, 6), "National Day");
        gothenburg.AddHoliday(new DateTime(2013, 6, 21), "Midsummer Eve");
        gothenburg.AddHoliday(new DateTime(2013, 12, 24), "Christmas Eve");
        gothenburg.AddHoliday(new DateTime(2013, 12, 25), "Christmas Day");
        gothenburg.AddHoliday(new DateTime(2013, 12, 26), "Boxing Day");
        gothenburg.AddHoliday(new DateTime(2013, 12, 31), "New Year's Eve");
        _context.Cities.Add(gothenburg);
        _context.SaveChanges();
    }
}
