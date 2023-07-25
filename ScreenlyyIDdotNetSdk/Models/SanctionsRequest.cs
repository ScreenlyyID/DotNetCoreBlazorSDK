using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ScreenlyyIDdotNetSdk.Models;

public class SanctionsRequest
{
    [JsonProperty("man")]
    public string AccountName { get; set; }
        
    [JsonProperty("bmn")]
    public string MiddleName { get; set; }
        
    [JsonProperty("bln")]
    [Required]
    public string LastName { get; set; }
        
    [JsonProperty("bfn")]
    [Required]
    public string FirstName { get; set; }
        
    // TODO: what is the best type for this?
    [JsonProperty("dob")]
    public string DateOfBirth { get; set; }
        
    [JsonProperty("bsn")]
    public string Street { get; set; }
        
    [JsonProperty("bc")]
    public string City { get; set; }
        
    [JsonProperty("bs")]
    public string State { get; set; }
        
    [JsonProperty("bz")]
    public string Zip { get; set; }
        
    [JsonProperty("bco")]
    public string CountryCode { get; set; }
}