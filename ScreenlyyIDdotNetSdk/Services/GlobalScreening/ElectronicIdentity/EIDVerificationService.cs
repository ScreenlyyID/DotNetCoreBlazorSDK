using System.Text;
using Newtonsoft.Json;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening.ElectronicIdentity;

public class EIDVerificationService : IEIDVerificationService
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
    public EIDVerificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task EIDVerify1X1Async(PersonalDataViewModel personalData, string correlationId)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var jsonRequest = JsonConvert.SerializeObject(new EIDValidateRequestModel()
        {
            Address = new GlobalDataAddressRequest()
            {
                AddressLine1 = personalData.AddressLine1,
                AddressLine2 = personalData.AddressLine2,
                PostalCode = personalData.ZipCode,
                CountryCode = personalData.Country,
                Locality = personalData.City
            },
            Email = new GlobalDataEmailRequest()
            {
                FullEmailAddress = personalData.Email
            },
            Phone = new GlobalDataPhoneRequest()
            {
                PhoneNumber = personalData.PhoneNumber
            },
            Identity = new GlobalDataIdentityRequest()
            {
                Completename = personalData.CompletedName,
                SurnameFirst = personalData.LastName,
                Givenfullname = personalData.FirstName,
                Dob = personalData.DateOfBirth?.ToString("MM/dd/yyyy")
            }
        }, Formatting.Indented, _jsonSerializerSettings);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}/api/v1/eid-validate-1x1";
        _ = await _httpClient.PostAsync(url, stringContent);
    }

    public async Task EIDVerify2X2Async(PersonalDataViewModel personalData, string correlationId)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var jsonRequest = JsonConvert.SerializeObject(new EIDValidateRequestModel()
        {
            Address = new GlobalDataAddressRequest()
            {
                AddressLine1 = personalData.AddressLine1,
                AddressLine2 = personalData.AddressLine2,
                PostalCode = personalData.ZipCode,
                CountryCode = personalData.Country,
                Locality = personalData.City
            },
            Email = new GlobalDataEmailRequest()
            {
                FullEmailAddress = personalData.Email
            },
            Phone = new GlobalDataPhoneRequest()
            {
                PhoneNumber = personalData.PhoneNumber
            },
            Identity = new GlobalDataIdentityRequest()
            {
                Completename = personalData.CompletedName,
                SurnameFirst = personalData.LastName,
                Givenfullname = personalData.FirstName,
                Dob = personalData.DateOfBirth?.ToString("MM/dd/yyyy")
            }
        }, Formatting.Indented, _jsonSerializerSettings);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}/api/v1/eid-validate-2x2";
        _ = await _httpClient.PostAsync(url, stringContent);
    }
}

public class EIDValidateRequestModel
{
    public GlobalDataAddressRequest Address { get; set; }

    public GlobalDataIdentityRequest Identity { get; set; }

    public GlobalDataPhoneRequest Phone { get; set; }

    public GlobalDataEmailRequest Email { get; set; }
}

public class GlobalDataAddressRequest
{
    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string HouseNumber { get; set; }

    public string HouseNumberAddition { get; set; }

    public string Thoroughfare { get; set; }

    public string Locality { get; set; }

    public string PostalCode { get; set; }

    public string Province { get; set; }

    public string CountryCode { get; set; }
}

public class GlobalDataCredentials
{
    public string Tenant { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}

public class GlobalDataEmailRequest
{
    public string FullEmailAddress { get; set; }
}

public class GlobalDataIdentityRequest
{
    public string Completename { get; set; }

    public string Givenfullname { get; set; }

    public string SurnameFirst { get; set; }

    public string Nationalid { get; set; }

    public string Dob { get; set; }
}

public class GlobalDataPhoneRequest
{
    public string PhoneNumber { get; set; }
}