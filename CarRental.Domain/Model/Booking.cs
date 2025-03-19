namespace CarRental.Domain.Model;

public class Booking
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string BookingNumber { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid? CustomerId { get; private set; } // CustomerId should be nullable in case of GDPR wipe. 
    public DateTime BookedDateTime { get; private set; }
    public DateTime? ReturnedDateTime { get; private set; } = null;
    public uint BookedVehicleMileage { get; private set; }
    public uint? ReturnedVehicleMileage { get; private set; } = null;
    
    public decimal? TotalCost { get; private set; }
    public byte[] RowVersion { get; private set; }

    // Empty constructor for EF-Core
    private Booking() {}
    
    public Booking(string bookingNumber, Guid vehicleId, Guid customerId, DateTime bookedDateTime, uint bookedVehicleMileage)
    {
        BookingNumber = bookingNumber;
        VehicleId = vehicleId;
        CustomerId = customerId;
        BookedDateTime = bookedDateTime;
        BookedVehicleMileage = bookedVehicleMileage;
    }

    public void ReturnVehicle(uint mileage)
    {
        ReturnedVehicleMileage = mileage;
        ReturnedDateTime = DateTime.UtcNow;
    }

    public void AddTotalCost(decimal totalCost)
    {
        TotalCost = totalCost;
    }
}