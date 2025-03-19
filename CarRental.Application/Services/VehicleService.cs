using CarRental.Application.DTOs.Vehicle;
using CarRental.Application.Mappings.VehicleMappings;
using CarRental.Domain.Repositories;

namespace CarRental.Application.Services;


public interface IVehicleService
{
    Task<VehicleDTO> GetVehicleById(Guid id);
    Task<List<VehicleDTO>> GetAllVehicles();
    Task<VehicleDTO> AddVehicle(CreateVehicleDTO vehicle);
    Task DeleteVehicle(Guid id);
}

public class VehicleService  : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    
    public VehicleService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }
    
    public async Task<VehicleDTO> GetVehicleById(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        return vehicle == null ? null : VehicleMapper.ToDTO(vehicle);
    }

    public async Task<List<VehicleDTO>> GetAllVehicles()
    {
        var  vehicles = await _vehicleRepository.GetAllAsync();
        return vehicles.Select(VehicleMapper.ToDTO).ToList();
    }

    public async Task<VehicleDTO> AddVehicle(CreateVehicleDTO dto)
    {
        // Make sure that there is no duplicates of vehicles.
        if (await _vehicleRepository.GetByLicencePlateAsync(dto.LicencePlate) != null)
        {
            throw new InvalidOperationException($"A vehicle with license plate '{dto.LicencePlate}' already exists.");
        }
        if (! ValidLicensePlate(dto.LicencePlate))
        {
            throw new ArgumentException($"Invalid license plate '{dto.LicencePlate}' should be in format XXX000.");
        }
        
        var vehicleToCreate = VehicleMapper.FromCreateDTO(dto);
        await _vehicleRepository.AddAsync(vehicleToCreate);
        var createdVehicle = await _vehicleRepository.GetByIdAsync(vehicleToCreate.Id);
        return createdVehicle == null ? null : VehicleMapper.ToDTO(createdVehicle);
    }
    
    public static bool ValidLicensePlate(string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate) || licensePlate.Length != 6)
            return false;
        
        // First three characters should be uppercase letters
        for (int i = 0; i < 3; i++)
        {
            if (!char.IsUpper(licensePlate[i]) || !char.IsLetter(licensePlate[i]))
                return false;
        }
    
        // Last three characters should be digits
        for (int i = 3; i < 6; i++)
        {
            if (!char.IsDigit(licensePlate[i]))
                return false;
        }
    
        return true;
    }
    
    public async Task DeleteVehicle(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            return;
        if (vehicle.Deleted)
            return;
        

        vehicle.DeleteVehicle();
        await _vehicleRepository.DeleteAsync(vehicle);
    }
    

    
}