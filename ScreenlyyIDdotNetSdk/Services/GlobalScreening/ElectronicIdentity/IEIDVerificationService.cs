using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening.ElectronicIdentity;

public interface IEIDVerificationService
{
    Task EIDVerify1X1Async(PersonalDataViewModel personalData, string correlationId);

    Task EIDVerify2X2Async(PersonalDataViewModel personalData, string correlationId);
}