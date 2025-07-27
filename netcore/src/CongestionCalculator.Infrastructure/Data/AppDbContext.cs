using CongestionCalculator.Domain.Entities;
using CongestionCalculator.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CongestionCalculator.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CityConfiguration());
        modelBuilder.ApplyConfiguration(new HolidayConfiguration());
        modelBuilder.ApplyConfiguration(new TaxRuleConfiguration());
    }
}
