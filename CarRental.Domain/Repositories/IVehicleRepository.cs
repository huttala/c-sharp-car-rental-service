using CarRental.Domain.Enums;
using CarRental.Domain.Model;

namespace CarRental.Domain.Repositories;

public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllAsync(bool includeDeleted = false);
    Task<List<Vehicle>> GetAvailableAsync(VehicleType? vehicleType = null);
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<Vehicle?> GetByLicencePlateAsync(string licencePlate);
    Task AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle id);
}