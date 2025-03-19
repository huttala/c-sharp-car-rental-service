using CarRental.Application.DTOs.Booking;
using CarRental.Application.Services;
using FluentValidation;

namespace CarRental.Application.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDTO>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.LicencePlate)
            .NotEmpty().WithMessage("License plate is required")
            .Length(6).WithMessage("License plate must be 6 characters")
            .Must(VehicleService.ValidLicensePlate).WithMessage("License plate must be in format XXX000");
                
        RuleFor(x => x.CustomerPersonalNumber)
            .NotEmpty().WithMessage("Customer personal number is required")
            .Length(12).WithMessage("Personal number must be 12 digits")
            .Matches(@"^\d{12}$").WithMessage("Personal number must contain only digits");
    }
}