using FluentValidation;
using CarRental.Application.DTOs.Customer;

namespace CarRental.Application.Validators
{
    public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDTO>
    {
        public CreateCustomerDtoValidator()
        {
            RuleFor(x => x.PersonalNumber)
                .NotEmpty().WithMessage("Personal number is required")
                .Length(12).WithMessage("Personal number must be 12 digits")
                .Matches(@"^\d{12}$").WithMessage("Personal number must contain only digits");
                
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");
                
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");
                
            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("A valid email address is required");
                
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[0-9\+\-\s]*$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number must contain only digits, spaces, plus or minus signs");
        }
    }
}