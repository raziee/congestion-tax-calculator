using CongestionCalculator.Application.Interfaces;
using CongestionCalculator.Domain.Enums;

namespace CongestionCalculator.Application;

public class TaxCalculatorService(ICityRepository ـcityRepository)
{
    public decimal CalculateTax(VehicleType vehicleType, DateTime[] passages, string cityName)
    {
        var city = ـcityRepository.GetCityByName(cityName);
        if (city is null)
            throw new Exception("City not found");

        if (city.IsExemptVehicle(vehicleType))
            return 0;

        var passagesByDay = passages.GroupBy(p => p.Date);
        decimal totalTax = 0;
        foreach (var group in passagesByDay)
        {
            if (city.IsIsNonTaxableDay(group.Key))
                continue;

            var ordered = group.Select(x => x.TimeOfDay).OrderBy(x => x).ToArray();
            decimal dailyTax = 0;

            var windowStart = ordered[0];
            var maxFee = city.GetTaxFee(windowStart);
            if (ordered.Length == 1)
            {
                dailyTax = maxFee;
            }
            else
            {
                for (int i = 1; i < ordered.Length; i++)
                {
                    if (
                        city.SingleChargeRule
                        && (ordered[i] - windowStart).TotalMinutes <= city.SingleChargeMinute
                    )
                    {
                        var fee = city.GetTaxFee(ordered[i]);
                        maxFee = Math.Max(maxFee, fee);
                    }
                    else
                    {
                        dailyTax += maxFee;
                        windowStart = ordered[i];
                        maxFee = city.GetTaxFee(windowStart);
                    }

                    if (i == ordered.Length - 1)
                        dailyTax += maxFee;
                }
            }
            totalTax += Math.Min(dailyTax, city.MaxDailyTax);
        }
        return totalTax;
    }
}
