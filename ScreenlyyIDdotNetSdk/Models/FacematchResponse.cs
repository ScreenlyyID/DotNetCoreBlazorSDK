namespace ScreenlyyIDdotNetSdk.Models;

public class FacematchResponse
{
    public int Score { get; set; }
    public bool IsMatch { get; set; }
    public string TransactionId { get; set; }
}