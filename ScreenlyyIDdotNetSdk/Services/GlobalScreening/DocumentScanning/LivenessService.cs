using System.Text;
using Newtonsoft.Json;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening.DocumentScanning;

public class LivenessService : ILivenessService
{
    /// <summary>
    /// Local variables
    /// </summary>
    private readonly HttpClient _httpClient;
    private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;
    

    /// <summary>
    /// class contructor
    /// </summary>
    /// <param name="httpClient">Http client DI</param>
    public LivenessService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PassiveLivenessResponse> ProcessLiveness(string image, string correlationId)
    {
        PassiveLivenessResponse response = null;
        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
            _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

            var body = new PassLiveRequest()
            {
                Image = image
            };
             
            var jsonRequest = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            string url = $"{baseUrl}/api/v1/liveness";

          
            var plResponse = await _httpClient.PostAsync(url, stringContent);
            var content = await plResponse.Content.ReadAsStringAsync();

            response = JsonConvert.DeserializeObject<PassiveLivenessResponse>(content); //TODO null check
            // should be saved to be used later
            var obj = content;
        }
        catch (Exception ex)
        {
            throw;
        }

        // return
        return response;
    }
}

public class PassLiveRequest
{
    public Settings Settings { get; set; } = new Settings();
        
    public string Image { get; set; }
}

public class Settings
{
    public string SubscriptionId { get; set; }
        
    public Dictionary<string, string> AdditionalSettings { get; set; } = new Dictionary<string, string>();
}