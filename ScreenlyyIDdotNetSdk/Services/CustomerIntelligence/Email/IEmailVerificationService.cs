using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Email;

public interface IEmailVerificationService
{
    Task VerifyAsync(PersonalDataViewModel personalData, string correlationId);
}