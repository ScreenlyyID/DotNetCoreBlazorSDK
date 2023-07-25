using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Phone;

public interface IPhoneVerificationService
{
    Task ValidateAsync(PersonalDataViewModel personalData, string correlationId);

    Task HLRLookupAsync(PersonalDataViewModel personalData, string correlationId);
}