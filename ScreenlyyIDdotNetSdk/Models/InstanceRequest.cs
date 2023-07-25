namespace ScreenlyyIDdotNetSdk.Models;

public class InstanceRequest
{
    public string? FirstName { get; set; }
        
    public string? LastName { get; set; }
        
    public Guid? ClientLookupId { get; set; }
        
    public List<string> GlobalScreening { get; set; }
        
    public List<string> CustomerIntelligence { get; set; }
}