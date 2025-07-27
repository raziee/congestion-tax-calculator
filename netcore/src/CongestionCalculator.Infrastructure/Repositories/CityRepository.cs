using CongestionCalculator.Application.Interfaces;
using CongestionCalculator.Domain.Entities;
using CongestionCalculator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CongestionCalculator.Infrastructure.Repositories;

public class CityRepository(AppDbContext _context) : ICityRepository
{
    public City GetCityByName(string name)
    {
        return _context
                .Cities
                .AsNoTracking()
                .Include(c => c.Holidays)
                .Include(c => c.TaxRules)
                .FirstOrDefault(c => c.Name == name);
    }
}
