using WebApplication1;
using Newtonsoft.Json.Linq;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;       
using System.Collections.Generic;          
using System.Threading.Tasks;            
public class ServiceMap
{
    private readonly MyDbContextMap _context;
    public ServiceMap(MyDbContextMap context)
    { _context = context; }

    public async Task<List<object>> GetAllDevice()
    {
        var maps = await _context.Devices
            .Select(r => new
             {
                 r.DeviceID,
                 r.DeviceUUID,
                 r.Nickname
             })
            .ToListAsync();
        return maps.Cast<object>().ToList();
    }
    public async Task<List<object>> GetAllMap()
    {
        var maps = await _context.IncidentReports
            .Where(r => !(r.IsDeleted ?? false))
            .Select(r => new
            {
                r.ReportID,
                r.Latitude,
                r.Longitude,
                r.SeverityLevel,
                r.IsResolved,
                r.ReportTimestamp,
            })
            .ToListAsync();

        return maps.Cast<object>().ToList();
    }
    public async Task<List<object>> GetAllMapbyUUID(string UUID)
    {
        var deviceId = await _context.Devices
            .Where(d => d.DeviceUUID == UUID)
            .Select(d => d.DeviceID)
            .FirstOrDefaultAsync();

        var maps = await _context.IncidentReports
            .Where(r => !(r.IsDeleted ?? false) && r.DeviceID == deviceId)
            .Select(r => new
            {
                r.ReportID,
                r.Latitude,
                r.Longitude,
                r.SeverityLevel,
                r.IsResolved,
                r.ReportTimestamp,
            })
            .ToListAsync();

        return maps.Cast<object>().ToList();
    }
    public async Task<List<object>> GetNickname(string UUID)
    {
        var name = await _context.Devices
            .Where(r => r.DeviceUUID == UUID)
            .Select(r => new
            {
                r.Nickname,
            })
            .ToListAsync();

        return name.Cast<object>().ToList();
    }



    public async Task<List<object>> GetAllMapbyID(int ID)
    {
        var maps = await _context.IncidentReports
            .Where(r => !(r.IsDeleted ?? false) && r.ReportID == ID)
            .Select(r => new
            {
                r.ReportID,
                r.Latitude,
                r.Longitude,
                r.IncidentType,
                r.SeverityLevel,
                r.IsResolved,
                r.UserDescription,
                r.DeleteReason,
                Nickname = _context.Devices
                    .Where(d => d.DeviceID == r.DeviceID)
                    .Select(d => d.Nickname)
                    .FirstOrDefault(),
                r.ReportTimestamp,
                IncidentImages = r.IncidentImages.Select(i => new
                {
                    i.ImageID,
                    i.ImageData
                }).ToList(),
                Charts = _context.Charts
                    .Where(c => !(c.IsDeleted ?? false) && c.ReportID == r.ReportID)
                    .Select(c => new
                    {
                        c.Data,
                        c.CreatedAt,
                        Nickname = _context.Devices
                            .Where(d => d.DeviceID == c.DeviceID)
                            .Select(d => d.Nickname)
                            .FirstOrDefault()
                    }).ToList()
            })
            .ToListAsync();

        return maps.Cast<object>().ToList();
    }
    public async Task<IncidentReports> AddIncidentReport(JObject data)
    {
        var report = data.ToObject<IncidentReports>();
        var deviceUuid = data["DeviceUUID"]?.ToString();
        var device = await _context.Devices
            .FirstOrDefaultAsync(d => d.DeviceUUID == deviceUuid);

        if (device == null)
        {
            device = new Devices
            {
                DeviceUUID = deviceUuid,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
            };
     
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();
            
            
        }
        device.LastSeen = DateTime.UtcNow;
        report.DeviceID = device.DeviceID;
        report.ReportTimestamp = DateTime.UtcNow;

        await _context.IncidentReports.AddAsync(report);
        await _context.SaveChangesAsync();

        return report;
    }

    public async Task<IncidentImages?> AddIncidentImage(JObject data)
    {
        var reportId = data["ReportID"]?.ToObject<int>() ?? 0;
        var imageData = data["ImageData"]?.ToString();

        if (reportId <= 0 || string.IsNullOrEmpty(imageData)) return null;
        if (!await _context.IncidentReports.AnyAsync(r => r.ReportID == reportId)) return null;

        var image = new IncidentImages
        {
            ReportID = reportId,
            ImageData = Convert.FromBase64String(imageData),
            UploadedAt = DateTime.UtcNow
        };

        await _context.IncidentImages.AddAsync(image);
        await _context.SaveChangesAsync();

        return image;
    }
    public async Task<Charts> AddChart(JObject data)
    {
        var chart = data.ToObject<Charts>();
        var deviceUuid = data["DeviceUUID"]?.ToString();

        var device = await _context.Devices
            .FirstOrDefaultAsync(d => d.DeviceUUID == deviceUuid);

        if (device == null)
        {
            device = new Devices
            {
                DeviceUUID = deviceUuid,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
            };

            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();
        }

        device.LastSeen = DateTime.UtcNow;

        if (chart.ReportID.HasValue)
        {
            var reportExists = await _context.IncidentReports
                .AnyAsync(r => r.ReportID == chart.ReportID.Value);

            if (!reportExists)
            {
                throw new ArgumentException($"ReportID {chart.ReportID} does not exist.");
            }
        }

        chart.DeviceID = device.DeviceID;
        chart.CreatedAt = DateTime.UtcNow;
        chart.IsDeleted = false;

        await _context.Charts.AddAsync(chart);
        await _context.SaveChangesAsync();

        return chart;
    }
    public async Task<Devices> AddUserName(JObject data)
    {
        var deviceUuid = data["DeviceUUID"]?.ToString();
        var nickname = data["Nickname"]?.ToString();

        var device = await _context.Devices
            .FirstOrDefaultAsync(d => d.DeviceUUID == deviceUuid);   

        if (device == null)
        {
            device = new Devices
            {
                DeviceUUID = deviceUuid,
                Nickname = nickname,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
            };
            await _context.Devices.AddAsync(device);
        }
        else
        {
            device.Nickname = nickname;     
            device.LastSeen = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return device;
    }
    public async Task<IncidentReports> UpdateIncidentReport(int id, JObject data)
    {
        var report = data.ToObject<IncidentReports>();
        report.ReportID = id;            
        _context.IncidentReports.Update(report);
        await _context.SaveChangesAsync();
        return report;
    }
    public async Task<IncidentReports> SolveIncidentReport(int id)
    {
        var report = await _context.IncidentReports.FindAsync(id);
        report.IsResolved = true;
        await _context.SaveChangesAsync();
        return report;
    }
    public async Task<IncidentReports> unSolveIncidentReport(int id)
    {
        var report = await _context.IncidentReports.FindAsync(id);
        report.IsResolved = false;
        await _context.SaveChangesAsync();
        return report;
    }


    public async Task<IncidentReports> FakeIncidentReport(int id)
    {
        var report = await _context.IncidentReports.FindAsync(id);
        report.IsDeleted = true;
        report.DeleteReason = "Fake Report";
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task<IncidentReports> unFakeIncidentReport(int id)
    {
        var report = await _context.IncidentReports.FindAsync(id);
        report.IsDeleted = false;
        report.DeleteReason = "test";
        await _context.SaveChangesAsync();
        return report;
    }
}
