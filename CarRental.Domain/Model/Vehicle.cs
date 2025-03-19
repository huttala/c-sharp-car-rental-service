using CarRental.Domain.Enums;

namespace CarRental.Domain.Model;

public class Vehicle
{
    public Guid Id { get; private set; } =  Guid.NewGuid();
    public string LicencePlate { get; private set; } 
    public VehicleType VehicleType { get; private set; }  
    public VehicleStatus VehicleStatus { get; private set; } = Enums.VehicleStatus.Available;
    public uint Mileage { get; private set; } = 0;
    public bool Deleted { get; private set; } = false;
    public byte[] RowVersion { get; private set; }

    // Empty constructor for EF-Core to work. 
    private Vehicle() {}

    public Vehicle(string licencePlate, VehicleType vehicleType, uint mileage)
    {
        LicencePlate = licencePlate.ToUpper();
        VehicleType = vehicleType;
        Mileage = mileage;
    }

    public void UpdateMilage(uint mileage)
    {
        if (Mileage > mileage)
        {
            // TODO: Log this error in busines logic.
            throw new ArgumentException("Mileage cannot be updated to a lower value than it already has.");
        }
        
        Mileage = mileage;
    }

    public void UpdateVehicleStatus(VehicleStatus vehicleStatus)
    {
        VehicleStatus = vehicleStatus;
    }

    public void DeleteVehicle()
    {
        Deleted = true;
    }

    public void UnDeleteVehicle()
    {
        Deleted = false;
    }
}