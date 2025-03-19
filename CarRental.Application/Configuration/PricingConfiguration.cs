namespace CarRental.Application.Configuration;

public class PricingConfiguration
{
    public decimal BaseDailyPrice { get; set; }
    public decimal BaseKmPrice { get; set; }
    public PricingMultipliers Multipliers { get; set; } = new();
}

public class PricingMultipliers
{
    public VehicleTypeMultiplier SmallCar { get; set; } = new();
    public VehicleTypeMultiplier CombiCar { get; set; } = new();
    public VehicleTypeMultiplier Truck { get; set; } = new();
}

public class VehicleTypeMultiplier
{
    public decimal DailyRate { get; set; }
    public decimal KmRate { get; set; }
}