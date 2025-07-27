using CongestionCalculator.Domain.SeedWork;

namespace CongestionCalculator.Domain.Entities;

public class TaxRule : EntityBase<int>
{
    public TimeSpan StartTime { get; }
    public TimeSpan EndTime { get; }
    public decimal Amount { get; }

    private TaxRule() { }

    public TaxRule(TimeSpan startTime, TimeSpan endTime, decimal amount)
    {
        StartTime = startTime;
        EndTime = endTime;
        Amount = amount;
    }
}
