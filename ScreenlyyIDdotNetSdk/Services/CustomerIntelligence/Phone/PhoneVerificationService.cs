using System.Text;
using Newtonsoft.Json;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Phone;

public class PhoneVerificationService : IPhoneVerificationService
{
     /// <summary>
    /// Local variables
    /// </summary>
    private readonly HttpClient _httpClient;
    
    //private readonly string baseUrl = "https://localhost:38418";
    private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;
    
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// class contructor
    /// </summary>
    /// <param name="httpClient">Http client DI</param>
    public PhoneVerificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task HLRLookupAsync(PersonalDataViewModel personalData, string correlationId)
    {
        if (string.IsNullOrEmpty(personalData.PhoneNumber))
                return;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
            _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

            var jsonRequest = JsonConvert.SerializeObject(new PhoneVerificationRequest()
            {
                PhoneNumber = personalData.PhoneNumber,
                CountryCode = personalData.CountryCodePhone
            }, Formatting.Indented, _jsonSerializerSettings);
            var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var url = $"{baseUrl}/api/v1/hlr-lookup";
            _ = await _httpClient.PostAsync(url, stringContent);
        

    }

    public async Task ValidateAsync(PersonalDataViewModel personalData, string correlationId)
    {
        if (string.IsNullOrEmpty(personalData.PhoneNumber))
            return;

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var jsonRequest = JsonConvert.SerializeObject(new PhoneVerificationRequest()
        {
            PhoneNumber = personalData.PhoneNumber,
            CountryCode = personalData.CountryCodePhone
        }, Formatting.Indented, _jsonSerializerSettings);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}/api/v1/phone-validate";
        _ = await _httpClient.PostAsync(url, stringContent);
    }
}

public class PhoneVerificationRequest
{
    public string PhoneNumber { get; set; }

    public string CountryCode { get; set; }
}