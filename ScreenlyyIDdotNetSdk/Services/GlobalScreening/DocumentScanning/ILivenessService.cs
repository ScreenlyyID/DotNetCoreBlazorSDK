using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening.DocumentScanning;

public interface ILivenessService
{
    Task<PassiveLivenessResponse> ProcessLiveness(string image, string correlationId);
}