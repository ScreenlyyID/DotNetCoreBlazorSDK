namespace ScreenlyyIDdotNetSdk.Models;

public class DocumentResponse
{
    public IList<Alerts> Alerts { get; set; }
    public int AuthenticationSensitivity { get; set; }
    public Biographic Biographic { get; set; }
    public Classification Classification { get; set; }
    public IList<DataFields> DataFields { get; set; }
    public Device Device { get; set; }
    public string EngineVersion { get; set; }
    public IList<Fields> Fields { get; set; }
    public IList<Image> Images { get; set; }
    public string InstanceId { get; set; }
    public string LibraryVersion { get; set; }
    public int ProcessMode { get; set; }
    public IList<Region> Regions { get; set; }
    public int Result { get; set; }
    public Subscription Subscription { get; set; }
}

public class Alerts
{
    public string Actions { get; set; }
    public IList<string> DataFieldReferences { get; set; }
    public string Description { get; set; }
    public string Disposition { get; set; }
    public IList<string> FieldReferences { get; set; }
    public string Id { get; set; }
    public IList<string> ImageReferences { get; set; }
    public string Information { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public IList<string> RegionalReferences { get; set; }
    public int Result { get; set; }
}

public class Biographic
{
    public int? Age { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? FullName { get; set; }
    public int? Gender { get; set; }
    public string? Photo { get; set; }
}


    public class Classification
    {
        public ClassificationDetails ClassificationDetails { get; set; }
        public int Mode { get; set; }
        public bool OrientationChanged { get; set; }
        public bool PresentationChanged { get; set; }
        public Type Type { get; set; }
    }

    public class DataFields
    {
        public int DataSource { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public bool IsImage { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public RegionOfInterest RegionOfInterest { get; set; }
        public string RegionReference { get; set; }
        public decimal Reliability { get; set; }
        public string Type { get; set; }
       // public Value Value { get; set; }
        public string? Value { get; set; }
    }

    public class RegionOfInterest
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Value
    {
        public DocumentAlert DocumentAlert { get; set; }
    }

    public class DocumentAlert
    {
        public string Actions { get; set; }
        public IList<string> DataFieldReference { get; set; }
        public string Description { get; set; }
        public string Disposition { get; set; }
        public IList<string> FieldReferences { get; set; }
        public string Id { get; set; }
        public IList<string> ImageReferences { get; set; }
        public string Information { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public IList<string> RegionReferences { get; set; }
        public int Result { get; set; }
    }

    public class Device
    {
        public bool HasContactlessChipReader { get; set; }
        public bool HasMagneticStripeReader { get; set; }
        public string SerialNumber { get; set; }
        public DeviceType Type { get; set; }
    }

    public class DeviceType
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int SensorType { get; set; }
    }

    public class Fields
    {
        public IList<string> DataFieldReferences { get; set; }
        public int DataSource { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public bool IsImage { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string RegionReference { get; set; }
        public string Type { get; set; }
      //  public Value Value {  get; set;  }
      public string Value {  get; set;  }
    }

    public class Image
    {
        public int GlareMetric { get; set; }
        public int HorizontalResolution { get; set; }
        public string Id { get; set; }
        public bool IsCropped { get; set; }
        public bool IsTampered { get; set; }
        public int Light { get; set; }
        public string MimeType { get; set; }
        public int SharpnessMetric { get; set; }
        public int Side { get; set; }
        public string Uri { get; set; }
        public int VerticalResolution { get; set; }
    }

    public class Region
    {
        public int DocumentElement { get; set; }
        public string Id { get; set; }
        public string ImageReference { get; set; }
        public string Key { get; set; }
        public RegionOfInterest Rectangle { get; set; }
    }

    public class Subscription
    {
        public int DocumentProcessMode { get; set; }
        public string Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDevelopment { get; set; }
        public bool IsTrial { get; set; }
        public string Name { get; set; }
        public bool StorePII { get; set; }
    }