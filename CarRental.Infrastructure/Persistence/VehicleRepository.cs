using CarRental.Domain.Enums;
using CarRental.Domain.Model;
using CarRental.Domain.Repositories;
using CarRental.Infrastructure.DbConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Persistence;

public class VehicleRepository : IVehicleRepository
{
    private readonly CarRentalContext _context;

    public VehicleRepository(CarRentalContext context)
    {
        _context = context;
    }

    public async Task<List<Vehicle>> GetAllAsync(bool includeDeleted = false)
    {
        return await _context.Vehicles
            .Where(v => includeDeleted || !v.Deleted)
            .AsNoTracking() // No tracking needed when just fetching a list of data, saves memory
            .ToListAsync();
    }

    public async Task<List<Vehicle>> GetAvailableAsync(VehicleType? vehicleType = null)
    {
        var query = _context.Vehicles
            .Where(x => !x.Deleted && x.VehicleStatus.Equals(VehicleStatus.Available));

        if (vehicleType != null)
        {
            query = query.Where(x => x.VehicleType.Equals(vehicleType));
        }

        return await query.AsNoTracking()
                          .ToListAsync();
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _context.Vehicles.FindAsync(id);
    }

    public async Task<Vehicle?> GetByLicencePlateAsync(string licencePlate)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.LicencePlate == licencePlate.ToUpper() && !v.Deleted);
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        try
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Get the entity that caused the concurrency issue
            var entry = ex.Entries.Single();
            var databaseValues = await entry.GetDatabaseValuesAsync();
        
            if (databaseValues == null)
            {
                throw new Exception("The vehicle no longer exists in the database.");
            }
        
            throw new Exception($"The vehicle with id {vehicle.Id} has been modified by another user. Please refresh and try again.", ex);
        }
    }

    public async Task DeleteAsync(Vehicle vehicle)
    {
        try
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Get the current database values
            var entry = ex.Entries.Single();
            var databaseValues = await entry.GetDatabaseValuesAsync();
        
            if (databaseValues == null)
            {
                // VehicleMappings has been completely removed from the database
                // This should never happen inside the application since there is no hard deletes, but goal achieved I guess.
                return; 
            }
            // Check if the vehicle is already in the desired state (soft deleted)
            var databaseVehicle = (Vehicle)databaseValues.ToObject();
            if (databaseVehicle.Deleted)
            {
                return;
            }
            // There actually was a concurrency issue that
            // prevented the delete operation but the entity is not in the desired state
            throw new Exception($"The vehicle with id {vehicle.Id} was modified by another user. Please refresh and try again.", ex);
        }
    }
}