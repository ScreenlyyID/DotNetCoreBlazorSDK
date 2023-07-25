using System.Text;
using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening;

public class AMLService : IAMLService
{
    /// <summary>
    /// Local variables
    /// </summary>
    private readonly HttpClient _httpClient;
    private readonly string baseUrl = Constant.Constant.SYSTEM_API_URL;
    private readonly ILogger<AMLService> _logger;

    public AMLService(HttpClient httpClient, ILogger<AMLService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> CompleteAMLCheck(PersonalDataViewModel personalData, string correlationId)
    {
        //TODO optimizations, is copying object adding mem overhead.
        var request = new SanctionsRequest()
        {
            AccountName = "",
            City = personalData.City,
            CountryCode = personalData.Country,
            DateOfBirth = personalData.DateOfBirth?.ToString("MM/dd/yyyy"),
            FirstName = personalData.FirstName,
            LastName = personalData.LastName,
            MiddleName = personalData.MiddleName,
            State = personalData.State,
            Street = personalData.AddressLine1,
            Zip = personalData.ZipCode
        };

        try
        {
            string json = System.Text.Json.JsonSerializer.Serialize(request);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", Constant.Constant.API_KEY);
            _httpClient.DefaultRequestHeaders.Add("x-correlation-id", correlationId);

            var response = await _httpClient.PostAsync($"{this.baseUrl}/sanctions/match", stringContent);
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);

            return string.Empty;
        }
    }
}