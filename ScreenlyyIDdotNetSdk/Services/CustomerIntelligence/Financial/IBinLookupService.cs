using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Financial;

public interface IBinLookupService
{
    Task LookupAsync(PersonalDataViewModel personalData, string correlationId);
}