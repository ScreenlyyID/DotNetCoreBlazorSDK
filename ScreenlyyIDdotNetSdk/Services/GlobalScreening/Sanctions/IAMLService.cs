using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening;

public interface IAMLService
{
    Task<string> CompleteAMLCheck(PersonalDataViewModel personalData, string correlationId);
}