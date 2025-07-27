using CongestionCalculator.Domain.SeedWork;

namespace CongestionCalculator.Domain.Entities;

public class Holiday : EntityBase<int>
{
    public DateTime Date { get; }
    public string Description { get; }

    private Holiday() { }

    public Holiday(DateTime date, string description)
    {
        Date = date;
        Description = description;
    }
}
