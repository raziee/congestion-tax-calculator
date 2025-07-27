using CongestionCalculator.Domain.Entities;
using CongestionCalculator.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CongestionCalculator.Infrastructure.Data.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Currency).HasMaxLength(8).IsRequired();
        builder.Property(c => c.MaxDailyTax).IsRequired();
        builder.Property(c => c.SingleChargeRule);
        builder.Property(c => c.SingleChargeMinute).IsRequired();
        builder.Property(c => c.NoTaxBeforeHoliday);
        builder
            .Property(e => e.NonTaxableDayOfWeeks)
            .HasConversion(
                v => string.Join(',', v.Select(e => (int)e)),
                v =>
                    v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), e))
                        .ToList()
            )
            .HasColumnType("nvarchar(128)");

        builder
            .Property(e => e.NonTaxableMonths)
            .HasConversion(
                v => string.Join(',', v.Select(e => (int)e)),
                v =>
                    v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => (Month)Enum.Parse(typeof(Month), e))
                        .ToList()
            )
            .HasColumnType("nvarchar(128)");

        builder
            .Property(e => e.ExemptVehicles)
            .HasConversion(
                v => string.Join(',', v.Select(e => (int)e)),
                v =>
                    v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => (VehicleType)Enum.Parse(typeof(VehicleType), e))
                        .ToList()
            )
            .HasColumnType("nvarchar(128)");
    }
}
