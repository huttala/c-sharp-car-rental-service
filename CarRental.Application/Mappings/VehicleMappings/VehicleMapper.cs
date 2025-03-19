using CarRental.Application.DTOs.Vehicle;
using CarRental.Domain.Model;

namespace CarRental.Application.Mappings.VehicleMappings;

public static class VehicleMapper
{
    public static VehicleDTO ToDTO(Vehicle vehicle)
    {
        return new VehicleDTO 
        {
            Id = vehicle.Id,
            LicencePlate = vehicle.LicencePlate,
            VehicleType = vehicle.VehicleType,
            VehicleStatus = vehicle.VehicleStatus,
            Mileage = vehicle.Mileage
        };
    }

    
    public static Vehicle FromCreateDTO(CreateVehicleDTO dto)
    {
        return new Vehicle(dto.LicencePlate, dto.VehicleType, dto.Mileage);
    }
}