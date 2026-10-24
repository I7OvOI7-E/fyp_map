using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Models;

namespace WebApplication1
{
    public class MyDbContextMap : DbContext
    {

        public MyDbContextMap(DbContextOptions<MyDbContextMap> options) : base(options)
        {
        }

        public DbSet<IncidentReports> IncidentReports { get; set; }
        public DbSet<IncidentImages> IncidentImages { get; set; }
        public DbSet<Charts> Charts { get; set; }
        public DbSet<Devices> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContextMap).Assembly);
    }
    }

    public class IncidentReportsConfig : IEntityTypeConfiguration<IncidentReports>
    {
        public void Configure(EntityTypeBuilder<IncidentReports> builder)
        {
            builder.HasKey(e => e.ReportID);
            builder.Property(e => e.ReportID).ValueGeneratedOnAdd();
            builder.Property(e => e.ReportID).IsRequired();
            builder.Property(e => e.DeviceID).IsRequired();
            builder.Property(e => e.Latitude).HasPrecision(11, 8);
            builder.Property(e => e.Latitude).IsRequired();
            builder.Property(e => e.Longitude).HasPrecision(11, 8);
            builder.Property(e => e.Longitude).IsRequired();
            builder.Property(e => e.IncidentType).HasMaxLength(255);
            builder.Property(e => e.UserDescription).HasMaxLength(255);
            builder.Property(e => e.DeleteReason).HasMaxLength(255);
            builder.HasMany(e => e.IncidentImages)
                   .WithOne(e => e.IncidentReports)
                   .HasForeignKey(e => e.ReportID)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.Charts)
                   .WithOne(e => e.IncidentReports)
                   .HasForeignKey(e => e.ReportID)
                   .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(e => e.Devices)
                   .WithMany(e => e.IncidentReports)
                   .HasForeignKey(e => e.DeviceID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class IncidentImagesConfig : IEntityTypeConfiguration<IncidentImages>
    {
        public void Configure(EntityTypeBuilder<IncidentImages> builder)
        {
            builder.HasKey(e => e.ImageID);
            builder.Property(e => e.ImageID).ValueGeneratedOnAdd();
            builder.Property(e => e.ImageID).IsRequired();
            builder.Property(e => e.ReportID).IsRequired();
            builder.Property(e => e.ImageData).IsRequired();
            builder.HasOne(e => e.IncidentReports)
                   .WithMany(e => e.IncidentImages)
                   .HasForeignKey(e => e.ReportID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ChartsConfig : IEntityTypeConfiguration<Charts>
    {
        public void Configure(EntityTypeBuilder<Charts> builder)
        {
            builder.HasKey(e => e.ChartID);
            builder.Property(e => e.ChartID).ValueGeneratedOnAdd();
            builder.Property(e => e.ChartID).IsRequired();
            builder.Property(e => e.Data).HasMaxLength(255);
            builder.HasOne(e => e.IncidentReports)
                   .WithMany(e => e.Charts)
                   .HasForeignKey(e => e.ReportID)
                   .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(e => e.Devices)
                   .WithMany(e => e.Charts)
                   .HasForeignKey(e => e.DeviceID)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class DevicesConfig : IEntityTypeConfiguration<Devices>
    {
        public void Configure(EntityTypeBuilder<Devices> builder)
        {
            builder.HasKey(e => e.DeviceID);
            builder.Property(e => e.DeviceID).ValueGeneratedOnAdd();
            builder.Property(e => e.DeviceID).IsRequired();
            builder.Property(e => e.DeviceUUID).HasMaxLength(255);
            builder.Property(e => e.Nickname).HasMaxLength(255);
            builder.HasMany(e => e.IncidentReports)
                   .WithOne(e => e.Devices)
                   .HasForeignKey(e => e.DeviceID)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.Charts)
                   .WithOne(e => e.Devices)
                   .HasForeignKey(e => e.DeviceID)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

}