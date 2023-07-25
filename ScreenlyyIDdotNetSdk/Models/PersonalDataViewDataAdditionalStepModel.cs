using System.ComponentModel.DataAnnotations;
using IntlTelInputBlazor;
using IntlTelInputBlazor.Validation;

namespace ScreenlyyIDdotNetSdk.Models;

public class PersonalDataViewDataAdditionalStepModel
{
    [EmailAddress]
    public string Email { get; set; }
    
    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }
    
    public string City { get; set; }

    public string State { get; set; }
    
    public string ZipCode { get; set; }

    public string CountryCode { get; set; }

    public string Country
    {
        get;
        set;
    }

    public string PhoneNumber { get; set; }

    public string BinNumber { get; set; }

    [IntlTelephone(ErrorMessage = "Phone number incorrect format")]
    public IntlTel IntTelNumber { get; set; }
}