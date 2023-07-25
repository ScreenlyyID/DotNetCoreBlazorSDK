using System.Text;
using Newtonsoft.Json;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening.DocumentScanning;

public class FacematchService : IFacematchService
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
    public FacematchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<FacematchResponse> ProcessFaceMatch(string selfie, string idPhoto, string correlationId)
    {
        FacematchResponse result;
        
        // To process the facematch API you need 2 images:
        // 1. The image on the original ID that was scanned - this can be retrieved using the instance ID: 
        // 2. The selfie image just taken. So the selfie needs to be re-used here.
        var request = new FaceMatchRequest()
        {
            Settings = new Settings(),
            Data = new ImageData()
            {
                ImageOne = selfie,
                ImageTwo = idPhoto
            }
        };
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
        _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);
				
        
        var jsonRequest = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        
        var url = $"{this.baseUrl}/api/v1/facematch";
        var response = await _httpClient.PostAsync(url, stringContent);
        var content = await response.Content.ReadAsStringAsync();

        result = JsonConvert.DeserializeObject<FacematchResponse>(content); //TODO null check

        return result;

        //  throw new NotImplementedException();
    }
}

public class FaceMatchRequest
{
    public Settings Settings { get; set; } = new Settings();
        
    public ImageData Data { get; set; } = new ImageData();
}

public class ImageData
{
    public string ImageOne { get; set; }
        
    public string ImageTwo { get; set; }
}