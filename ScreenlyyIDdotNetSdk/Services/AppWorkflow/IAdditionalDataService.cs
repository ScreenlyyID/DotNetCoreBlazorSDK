using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.AppWorkflow;

public interface IAdditionalDataService
{
    Task SaveAdditionalData(PersonalDataViewModel personalDataViewModel, string instanceId, string correlationId);
}