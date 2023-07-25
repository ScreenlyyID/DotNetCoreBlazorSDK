using System.Text;
using Newtonsoft.Json;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Email;

public class EmailVerificationService : IEmailVerificationService
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
    public EmailVerificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task VerifyAsync(PersonalDataViewModel personalData, string correlationId)
    {
        if (string.IsNullOrEmpty(personalData.Email))
            return;

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var jsonRequest = JsonConvert.SerializeObject(new EmailVerificationRequest()
        {
            Email = personalData.Email
        }, _jsonSerializerSettings);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}/api/v1/email-verify";
        _ = await _httpClient.PostAsync(url, stringContent);
    }
}

public class EmailVerificationRequest
{
    [JsonProperty("email")]
    public string Email { get; set; }
}