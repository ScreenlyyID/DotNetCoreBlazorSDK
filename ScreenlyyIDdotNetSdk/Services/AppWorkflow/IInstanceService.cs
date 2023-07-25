namespace ScreenlyyIDdotNetSdk.Services.AppWorkflow;

public interface IInstanceService
{
    Task<string> GetCorrelationId();
}