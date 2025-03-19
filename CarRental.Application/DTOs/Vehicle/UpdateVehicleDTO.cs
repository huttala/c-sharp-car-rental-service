namespace CarRental.Application.DTOs.Vehicle;

public class UpdateVehicleDTO
{
    public required Guid Id { get; init; }
    public required uint Mileage { get; init; }
    public required bool Deleted { get; init; }
}