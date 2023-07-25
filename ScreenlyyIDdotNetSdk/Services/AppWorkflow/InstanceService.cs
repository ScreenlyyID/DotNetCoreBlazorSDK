using System.Text;
using Newtonsoft.Json;
using ScreenlyyIDdotNetSdk.Constant;
using ScreenlyyIDdotNetSdk.Models;
using ScreenlyyIDdotNetSdk.Services.GlobalScreening.DocumentScanning;

namespace ScreenlyyIDdotNetSdk.Services.AppWorkflow;

public class InstanceService : IInstanceService
{
     private readonly HttpClient _httpClient;
    private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;
    private readonly ILogger<DocumentService> _logger;

    public InstanceService(HttpClient httpClient, ILogger<DocumentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<string> GetCorrelationId()
    {
        try
        {
            var instanceRequest = new InstanceRequest()
            {
                FirstName = null, // This is an optional value. It can be used if you plan on linking scans to the ScreenlyyID dashboard.
                LastName = null, // This is an optional value. It can be used if you plan on linking scans to the ScreenlyyID dashboard.
                ClientLookupId = null, // This is an optional value. It can be used if you plan on using the API to create distributable links from your own source.
                GlobalScreening = new List<string>()
                {
                    GlobalScreeningTypes.DocumentScan
                },
                CustomerIntelligence = new List<string>() {} // This optional, can be empty or determine a workflow for CustomerIntelligence checks if used in an external system
            };
            
            string json = System.Text.Json.JsonSerializer.Serialize(instanceRequest);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);

            var response = await _httpClient.PostAsync($"{this.baseUrl}/api/v1/instance/create", stringContent);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<InstanceResponse>(content); //TODO null check
            
            return result.CorrelationId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);

            return string.Empty;
        }
    }
}