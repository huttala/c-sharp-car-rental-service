using System.Runtime.InteropServices.ComTypes;
using CarRental.Domain.Enums;

namespace CarRental.Application.DTOs.Vehicle;

public class VehicleDTO
{
    public required Guid Id { get; init; }
    public required string LicencePlate { get; init; }
    public required VehicleType VehicleType { get; init; }  
    public required VehicleStatus VehicleStatus { get; init; }
    public required uint Mileage { get; init; }
}