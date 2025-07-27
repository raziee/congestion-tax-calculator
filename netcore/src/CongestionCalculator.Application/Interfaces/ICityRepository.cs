using CongestionCalculator.Domain.Entities;

namespace CongestionCalculator.Application.Interfaces;

public interface ICityRepository
{
    City GetCityByName(string name);
}
