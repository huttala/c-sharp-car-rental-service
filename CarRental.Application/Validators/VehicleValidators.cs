using CarRental.Application.DTOs.Vehicle;
using CarRental.Application.Services;
using FluentValidation;

namespace CarRental.Application.Validators;


public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDTO>
{
    public CreateVehicleDtoValidator()
    {
        RuleFor(x => x.LicencePlate)
            .NotEmpty().WithMessage("License plate is required")
            .Length(6).WithMessage("License plate must be 6 characters")
            .Must(VehicleService.ValidLicensePlate).WithMessage("License plate must be in format XXX000");
            
        RuleFor(x => x.VehicleType)
            .IsInEnum().WithMessage("Invalid vehicle type");
            
        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0u).WithMessage("Mileage cannot be negative");
    }
}
