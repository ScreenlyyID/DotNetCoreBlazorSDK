using System.ComponentModel.DataAnnotations;

namespace ScreenlyyIDdotNetSdk.Models;

public class PersonalDataViewDataInputStepModel
{
    
    public string FirstName { get; set; }
    
    public string MiddleName { get; set; }
    
    public string LastName { get; set; }
    
    public string DocumentNumber { get; set; }
    
    public DateTime? DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-18);

    public DateTime? DocumentExpiryDateTime { get; set; }

    public string Gender { get; set; }
}