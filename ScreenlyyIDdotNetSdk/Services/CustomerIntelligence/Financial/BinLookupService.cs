using System.Text;
using Newtonsoft.Json;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Financial;

public class BinLookupService : IBinLookupService
{
    /// <summary>
    /// Local variables
    /// </summary>
    private readonly HttpClient _httpClient;
    private readonly ILogger<BinLookupService> _logger;
    private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// class contructor
    /// </summary>
    /// <param name="httpClient">Http client DI</param>
    public BinLookupService(HttpClient httpClient, ILogger<BinLookupService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task LookupAsync(PersonalDataViewModel personalData, string correlationId)
    {
        try
        {
            if (string.IsNullOrEmpty(personalData.BinNumber))
                return;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
            _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

            var jsonRequest = JsonConvert.SerializeObject(new BinLookupRequest()
            {
                BinNumber = personalData.BinNumber,
            }, Formatting.Indented, _jsonSerializerSettings);
            var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var url = $"{baseUrl}/api/v1/bin-lookup";
            _ = await _httpClient.PostAsync(url, stringContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }

    }
}

public class BinLookupRequest
{
    public string BinNumber { get; set; }
}