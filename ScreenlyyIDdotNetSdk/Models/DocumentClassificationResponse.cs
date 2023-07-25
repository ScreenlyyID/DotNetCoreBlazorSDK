namespace ScreenlyyIDdotNetSdk.Models;

public class DocumentClassificationResponse
{
    public ClassificationDetails ClassificationDetails { get; set; }
    public int Mode { get; set; }
    public bool OrientationChanged { get; set; }
    public bool PresentationChanged { get; set; }
    public Type Type { get; set; }
}

public class ClassificationDetails
{
    public Back Back { get; set; }
    public Front Front { get; set; }
}

public class Back
{
    public int Class { get; set; }
    public string ClassCode { get; set; }
    public string ClassName { get; set; }
    public string CountryCode { get; set; }
    public IList<string> GeographicRegions { get; set; }
    public string Id { get; set; }
    public bool IsGeneric { get; set; }
    public string Issue { get; set; }
    public string IssueType { get; set; }
    public string IssuerCode { get; set; }
    public string IssuerName { get; set; }
    public int IssuerType { get; set; }
    public string KeesingCode { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
    public IList<SupportedImages> SupportedImages { get; set; }
}

public class SupportedImages
{
    public int Light { get; set; }
    public int Side { get; set; }
}

public class Front
{
    public int Class { get; set; }
    public string ClassCode { get; set; }
    public string ClassName { get; set; }
    public string CountryCode { get; set; }
    public IList<string> GeographicRegions { get; set; }
    public string Id { get; set; }
    public bool IsGeneric { get; set; }
    public string Issue { get; set; }
    public string IssueType { get; set; }
    public string IssuerCode { get; set; }
    public string IssuerName { get; set; }
    public int IssuerType { get; set; }
    public string KeesingCode { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
    public IList<SupportedImages> SupportedImages { get; set; }
}

public class Type
{
    public int Class { get; set; }
    public string ClassCode { get; set; }
    public string? ClassName { get; set; }
    public string CountryCode { get; set; }
    public IList<string> GeographicRegions { get; set; }
    public string Id { get; set; }
    public bool IsGeneric { get; set; }
    public string Issue { get; set; }
    public string IssueType { get; set; }
    public string IssuerCode { get; set; }
    public string IssuerName { get; set; }
    public int IssuerType { get; set; }
    public string KeesingCode { get; set; }
    public string? Name { get; set; }
    public int Size { get; set; }
    public IList<SupportedImages> SupportedImages { get; set; }
}