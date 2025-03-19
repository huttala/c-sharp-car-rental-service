namespace CarRental.Domain.Model;

public class Customer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string PersonalNumber { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? Email { get; private set; } = null; 
    public string? PhoneNumber { get; private set; } = null; 
    public bool IsDeleted { get; private set; } = false;
    public byte[] RowVersion { get; private set; }

    public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();
    
    // Empty constructor for EF-Core
    private Customer() {}


    public Customer(string personalNumber, string firstName, string lastName, string? email, string? phoneNumber)
    {
        PersonalNumber = personalNumber;
        FirstName = firstName;
        LastName = lastName;
        Email = email; // TODO: Add email validation in business logic later.
        PersonalNumber = personalNumber;
        PhoneNumber = phoneNumber; // TODO: Add phone number validation logic later. Swedish phone numbers only in the beginning.
    }

    public void DeleteCustomer()
    {
        PersonalNumber = "";
        FirstName = "";
        LastName = "";
        Email = null;
        PhoneNumber = null;
        IsDeleted = true;
    }
    
    public void UpdateFirstName(string firstName)
    {
        FirstName = firstName;
    }

    public void UpdateLastName(string lastName)
    {
        LastName = lastName;
    }

    public void UpdateEmail(string? email)
    {
        // TODO: Add email validation in business logic later.
        Email = email;
    }

    public void UpdatePhoneNumber(string? phoneNumber)
    {
        // TODO: Add phone number validation logic later. Swedish phone numbers only in the beginning.
        PhoneNumber = phoneNumber;
    }
    
    
}