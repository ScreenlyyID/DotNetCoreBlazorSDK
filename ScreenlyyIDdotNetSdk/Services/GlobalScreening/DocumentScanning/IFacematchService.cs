using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening.DocumentScanning;

public interface IFacematchService
{
    public Task<FacematchResponse> ProcessFaceMatch(string selfie, string idPhoto, string correlationId);
}