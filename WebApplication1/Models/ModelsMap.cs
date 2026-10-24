#nullable enable

namespace WebApplication1.Models
{
    public class IncidentReports
    {
        public int ReportID { get; set; } = 0;
        public int DeviceID { get; set; } = 0;
        public decimal Latitude { get; set; } = 0;
        public decimal Longitude { get; set; } = 0;
        public string? IncidentType { get; set; }
        public int? SeverityLevel { get; set; }
        public string? UserDescription { get; set; }
        public DateTime? ReportTimestamp { get; set; }
        public bool? IsResolved { get; set; }
        public bool? IsDeleted { get; set; }
        public string? DeleteReason { get; set; }
        public virtual ICollection<IncidentImages> IncidentImages { get; set; } = new List<IncidentImages>();
        public virtual ICollection<Charts> Charts { get; set; } = new List<Charts>();
        public virtual Devices? Devices { get; set; }
    }

    public class IncidentImages
    {
        public int ImageID { get; set; } = 0;
        public int ReportID { get; set; } = 0;
        public byte[] ImageData { get; set; }
        public DateTime? UploadedAt { get; set; }
        public virtual IncidentReports? IncidentReports { get; set; }
    }

    public class Charts
    {
        public int ChartID { get; set; } = 0;
        public int? DeviceID { get; set; }
        public int? ReportID { get; set; }
        public string? Data { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual IncidentReports? IncidentReports { get; set; }
        public virtual Devices? Devices { get; set; }
    }

    public class Devices
    {
        public int DeviceID { get; set; } = 0;
        public string DeviceUUID { get; set; } = string.Empty;
        public DateTime? FirstSeen { get; set; }
        public DateTime? LastSeen { get; set; }
        public string? Nickname { get; set; }
        public virtual ICollection<IncidentReports> IncidentReports { get; set; } = new List<IncidentReports>();
        public virtual ICollection<Charts> Charts { get; set; } = new List<Charts>();
    }

}