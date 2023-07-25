using Newtonsoft.Json;

namespace ScreenlyyIDdotNetSdk.Models;

public class PassiveLivenessResponse
{
    [JsonProperty("LivenessResult")]
    public LivenessResult LivenessResult { get; set; } = new LivenessResult();
        
    [JsonProperty("Error")]
    public string Error { get; set; }
        
    [JsonProperty("ErrorCode")]
    public string ErrorCode { get; set; }
        
    [JsonProperty("TransactionId")]
    public string TransactionId { get; set; }
}

public class LivenessResult
{
    [JsonProperty("Score")]
    public int Score { get; set; }
        
    [JsonProperty("LivenessAssessment")]
    public string LivenessAssessment { get; set; }
}