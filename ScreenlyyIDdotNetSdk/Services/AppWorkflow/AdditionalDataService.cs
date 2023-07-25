using System.Text;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.AppWorkflow;

public class AdditionalDataService : IAdditionalDataService
{
    private readonly HttpClient _httpClient;
    //private readonly string baseUrl = "https://localhost:38418";
    private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;
    private readonly ILogger<AdditionalDataService> _logger;
    
    public AdditionalDataService(HttpClient httpClient, ILogger<AdditionalDataService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task SaveAdditionalData(PersonalDataViewModel personalDataViewModel, string instanceId, string correlationId)
    {
        
        string json = System.Text.Json.JsonSerializer.Serialize(personalDataViewModel);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

        var response = await _httpClient.PostAsync($"{this.baseUrl}/api/v1/additional-data", stringContent);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Save additional data failed: " + response.StatusCode + " " +
                                response.RequestMessage);
        }
        var content = await response.Content.ReadAsStringAsync();
      
    }
}

