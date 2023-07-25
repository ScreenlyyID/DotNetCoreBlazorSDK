using ScreenlyyIDdotNetSdk.Models;

namespace ScreenlyyIDdotNetSdk.Services.GlobalScreening.DocumentScanning;

public interface IDocumentService
{
    Task<string> GetInstanceId(string correlationId);

    Task<DocumentClassificationResponse> GetClassification(string instanceId, string correlationId);

    Task<string> PostDocumentImage(string instanceId, string correlationId, int side, string imageTest);

    Task<string> GetDocumentImageField(string imageKey, string correlationId, string instanceId);

    Task<DocumentResponse> GetDocument(string correlationId, string instanceId);
}