using System.ComponentModel.DataAnnotations;
using IntlTelInputBlazor;
using IntlTelInputBlazor.Validation;

namespace ScreenlyyIDdotNetSdk.Models;

public class PersonalDataViewModel
{
    [MinLength(1, ErrorMessage = "First Name cannot be null")]
    public string FirstName { get; set; }


    public string MiddleName { get; set; }
    
    [MinLength(1, ErrorMessage = "Last Name cannot be null")]
    public string LastName { get; set; }

    public string DocumentNumber { get; set; }
    
    public DateTime? DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-18);

    public DateTime? DocumentExpiryDateTime { get; set; }

    public string Gender { get; set; }

    [EmailAddress]
    public string Email { get; set; }
    
    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }
    
    public string City { get; set; }
    
    public string State { get; set; }
    
    public string ZipCode { get; set; }

    public string CountryCodePhone { get; set; }

    public string Country { get; set; }

    public string PhoneNumber { get; set; }

    public string BinNumber { get;set;}

    [IntlTelephone(ErrorMessage = "Phone number incorrect format")]
    public IntlTel IntTelNumber { get; set; }

    public string CompletedName => $"{FirstName} {LastName}";
}