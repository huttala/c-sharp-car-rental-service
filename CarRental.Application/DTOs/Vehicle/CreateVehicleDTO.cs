using CarRental.Domain.Enums;

namespace CarRental.Application.DTOs.Vehicle;

public class CreateVehicleDTO
{
    public required string LicencePlate { get; init; }
    public required VehicleType VehicleType { get; init; }
    public required uint Mileage { get; init; } = 0;
}