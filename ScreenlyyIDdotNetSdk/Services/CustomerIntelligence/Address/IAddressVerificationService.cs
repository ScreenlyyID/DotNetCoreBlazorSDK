using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.CustomerIntelligence.Address;

public interface IAddressVerificationService
{
    Task AddressCleansePlusAsync(PersonalDataViewModel personalData, string correlationId);
}