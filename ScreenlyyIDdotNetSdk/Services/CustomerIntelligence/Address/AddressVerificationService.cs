using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Address;

public class AddressVerificationService : IAddressVerificationService
{
    /// <summary>
    /// Local variables
    /// </summary>
    private readonly HttpClient _httpClient;

    private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;

    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// class contructor
    /// </summary>
    /// <param name="httpClient">Http client DI</param>
    public AddressVerificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AddressCleansePlusAsync(PersonalDataViewModel personalData, string correlationId)
    
    {
        if (string.IsNullOrEmpty(personalData.Email))
            return;

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var jsonRequest = JsonConvert.SerializeObject(new AddressCleansePlusRequestModel()
        {
            Addresses = new List<AddressRequestInfoModel>()
            {
                new AddressRequestInfoModel()
                {
                    Address1 = personalData.AddressLine1,
                    Address2 = personalData.AddressLine2,
                    PostalCode = personalData.ZipCode,
                    Country = personalData.Country,
                    AdministrativeArea = personalData.State
                }
            }.ToArray()
        }, Formatting.Indented, _jsonSerializerSettings);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}/api/v1/address-cleanse";

        _ = await _httpClient.PostAsync(url, stringContent);
    }
}


public class AddressLookupRequest
{
    [Required]
    public string Text { get; set; }

    public string Container { get; set; }

    public string Countries { get; set; }

}
public class AddressVerificationRequest
{
    [Required]
    public string Id { get; set; }
}

public class AddressLookupResponse
{
    public AddressInfo[] AddressInfos { get; set; }
}

public partial class AddressInfo
{
    public string Id { get; set; }
}

public class AddressCleansePlusRequestModel
{
    public bool Geocode { get; set; }
    public AddressRequestInfoModel[] Addresses { get; set; }
}

public partial class AddressRequestInfoModel
{
    public string AddressAddress { get; set; }

    public string Address1 { get; set; }

    public string Address2 { get; set; }

    public string Address3 { get; set; }

    public string Address4 { get; set; }

    public string Address5 { get; set; }

    public string Address6 { get; set; }

    public string Address7 { get; set; }

    public string Address8 { get; set; }

    public string Country { get; set; }

    public string SuperAdministrativeArea { get; set; }

    public string AdministrativeArea { get; set; }

    public string SubAdministrativeArea { get; set; }

    public string Locality { get; set; }

    public string DependentLocality { get; set; }

    public string DoubleDependentLocality { get; set; }

    public string Thoroughfare { get; set; }

    public string DependentThoroughfare { get; set; }

    public string Building { get; set; }

    public string Premise { get; set; }

    public string SubBuilding { get; set; }

    public string PostalCode { get; set; }

    public string Organization { get; set; }

    public string PostBox { get; set; }
}
