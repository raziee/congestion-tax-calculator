using CongestionCalculator.Domain.Enums;
using CongestionCalculator.Domain.SeedWork;

namespace CongestionCalculator.Domain.Entities;

public class City : EntityBase<int>, IAggregateRoot
{
    private readonly List<TaxRule> _taxRules;
    private readonly List<Holiday> _holidays;

    public byte CountryId { get; }
    public string Name { get; }
    public decimal MaxDailyTax { get; }
    public bool SingleChargeRule { get; }
    public int SingleChargeMinute { get; }
    public bool NoTaxBeforeHoliday { get; }

    public string Currency { get; }
    public List<DayOfWeek> NonTaxableDayOfWeeks { get; } = new();
    public List<Month> NonTaxableMonths { get; } = new();
    public List<VehicleType> ExemptVehicles { get; } = new();
    public IReadOnlyCollection<TaxRule> TaxRules => _taxRules.AsReadOnly();
    public IReadOnlyCollection<Holiday> Holidays => _holidays.AsReadOnly();

    private City()
    {
        _taxRules = new List<TaxRule>();
        _holidays = new List<Holiday>();
    }

    public City(
        string name,
        string currency,
        decimal maxDailyTax,
        bool singleChargeRule,
        int singleChargeMinute,
        bool noTaxBeforeHoliday
    )
        : this()
    {
        Guard.For.NotNullOrEmpty(nameof(name), name);
        Guard.For.NotNullOrEmpty(nameof(currency), currency);

        Currency = currency;
        Name = name;
        MaxDailyTax = maxDailyTax;
        SingleChargeRule = singleChargeRule;
        SingleChargeMinute = singleChargeMinute;
        NoTaxBeforeHoliday = noTaxBeforeHoliday;
    }

    public void AddNonTaxableDayOfWeek(DayOfWeek dayOfWeek)
    {
        if (!NonTaxableDayOfWeeks.Contains(dayOfWeek))
            NonTaxableDayOfWeeks.Add(dayOfWeek);
    }

    public void AddNonTaxableMonth(Month month)
    {
        if (!NonTaxableMonths.Contains(month))
            NonTaxableMonths.Add(month);
    }

    public void AddTaxRule(TimeSpan startTime, TimeSpan endTime, decimal amount)
    {
        var taxRule = new TaxRule(startTime, endTime, amount);
        _taxRules.Add(taxRule);
    }

    public void AddExemptVehicle(VehicleType vehicleType)
    {
        if (!ExemptVehicles.Contains(vehicleType))
            ExemptVehicles.Add(vehicleType);
    }

    public bool IsExemptVehicle(VehicleType vehicleType)
    {
        return ExemptVehicles.Contains(vehicleType);
    }

    public void AddHoliday(DateTime date, string description)
    {
        var holiday = new Holiday(date, description);
        _holidays.Add(holiday);
    }

    public bool IsIsNonTaxableDay(DateTime date)
    {
        return NonTaxableDayOfWeeks.Contains(date.DayOfWeek)
            || NonTaxableMonths.Contains((Month)date.Month)
            || Holidays.Any(x => x.Date == date)
            || (NoTaxBeforeHoliday && Holidays.Any(x => x.Date == date.AddDays(1)));
    }

    public decimal GetTaxFee(TimeSpan time)
    {
        var rule = TaxRules.FirstOrDefault(x => time >= x.StartTime && time <= x.EndTime);
        return rule?.Amount ?? 0;
    }
}
