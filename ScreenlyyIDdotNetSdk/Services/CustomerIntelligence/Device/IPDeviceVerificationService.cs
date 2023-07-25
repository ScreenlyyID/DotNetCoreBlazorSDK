using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Device;

public class IPDeviceVerificationService : IIPDeviceVerificationService
{
    
    /// <summary>
    /// Local variables
    /// </summary>
    private readonly HttpClient _httpClient;
    
   
    //private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;
    private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;
    
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// class contructor
    /// </summary>
    /// <param name="httpClient">Http client DI</param>
    public IPDeviceVerificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task HostReputationAsync(string host, string correlationId)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var jsonRequest = JsonConvert.SerializeObject(new DeviceHostReputationRequestModel()
        {
            Host = host
        }, Formatting.Indented, _jsonSerializerSettings);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}/api/v1/host-reputation-verify";
        _ = await _httpClient.PostAsync(url, stringContent);
    }

    public async Task IPBlocklistAsync(string ipAdress, string correlationId)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var jsonRequest = JsonConvert.SerializeObject(new DeviceIPRequestModel()
        {
            IpAdress = ipAdress
        }, Formatting.Indented, _jsonSerializerSettings);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}/api/v1/ip-blocklist-verify";
        _ = await _httpClient.PostAsync(url, stringContent);
    }

    public async Task IPProbeAsync(string ipAdress, string correlationId)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var jsonRequest = JsonConvert.SerializeObject(new DeviceIPRequestModel()
        {
            IpAdress = ipAdress
        }, Formatting.Indented, _jsonSerializerSettings);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}/api/v1/ip-probe-verify";
        _ = await _httpClient.PostAsync(url, stringContent);
    }

    public async Task UALookupAsync(string userAgent, string correlationId)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var jsonRequest = JsonConvert.SerializeObject(new DeviceUALookupRequestModel()
        {
            UserAgent = userAgent
        }, Formatting.Indented, _jsonSerializerSettings);
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}/api/v1/ua-lookup-verify";
        _ = await _httpClient.PostAsync(url, stringContent);
    }
}


public class DeviceIPRequestModel
{
    [Required]
    public string IpAdress { get; set; }
}

public class DeviceHostReputationRequestModel
{
    [Required]
    public string Host { get; set; }
}
public class DeviceUALookupRequestModel
{
    [Required]
    public string UserAgent { get; set; }

    public string UserAgentVersion { get; set; }

    public string UserAgentFlatform { get; set; }

    public string UserAgentFlatformVersion { get; set; }

    public string UserAgentMobile { get; set; }

    public string DeviceModel { get; set; }

    public string DeviceBrand { get; set; }
}