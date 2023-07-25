namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Device;

public interface IIPDeviceVerificationService
{
    Task IPProbeAsync(string ipAdress, string correlationId);

    Task IPBlocklistAsync(string ipAdress, string correlationId);

    Task HostReputationAsync(string host, string correlationId);

    Task UALookupAsync(string userAgent, string correlationId);
}