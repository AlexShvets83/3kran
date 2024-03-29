﻿using DeviceDbModel.DataDb;
using DeviceDbModel.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace DeviceDbModel
{
  public class DeviceDBContextFactory : IDesignTimeDbContextFactory<DeviceDBContext>
  {
    private static string ConnectionString => MainSettings.Settings.ConnectionStrings.DefaultConnection;

    public DeviceDBContext CreateDbContext(string[] args)
    {
      IServiceCollection services = new ServiceCollection();

      services.AddDbContext<DeviceDBContext>(options => options.UseNpgsql(ConnectionString));
      services.Configure<OperationalStoreOptions>(_ => { });
      var context = services.BuildServiceProvider().GetService<DeviceDBContext>();
      return context;
    }

    public override string ToString() { return ConnectionString; }
  }

  public sealed partial class DeviceDBContext : ApiAuthorizationDbContext<ApplicationUser>
  {
    public DeviceDBContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions) { }

    public DbSet<Device> Devices { get; set; }

    public DbSet<UserDevicePermission> UserDevicePermissions { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<InviteRegistration> InviteRegistrations { get; set; }

    public DbSet<DevStatus> DeviceLastStatus { get; set; }

    public DbSet<DevErrorStatus> DeviceErrorStatus { get; set; }
    
    public DbSet<DevSale> DeviceSales { get; set; }

    public DbSet<DevEncash> DeviceEncashes { get; set; }

    public DbSet<DevAlert> DeviceAlerts { get; set; }

    public DbSet<DevInfo> DeviceInfos { get; set; }

    public DbSet<DevSetting> DeviceSettings { get; set; }

    public DbSet<LogUsr> UserLog { get; set; }

    public DbSet<FileModel> Files { get; set; }

    public DbSet<AppSettings> AppSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        //optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Device>(entity =>
      {
        entity.HasIndex(e => e.Imei).IsUnique();
        entity.HasIndex(e => e.Address);
        entity.HasOne(d => d.User).WithMany(p => p.Devices).HasForeignKey(d => d.OwnerId).OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<UserDevicePermission>(entity =>
      {
        entity.HasIndex(e => e.UserId);
        entity.HasOne(d => d.User).WithMany(p => p.UserDevicePermissions).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(d => d.Device).WithMany(p => p.UserDevicePermissions).HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade);
      });
      
      modelBuilder.Entity<ApplicationUser>(entity =>
      {
        entity.HasIndex(e => e.CountryId);
        entity.HasIndex(e => e.OwnerId);
        entity.Property(e => e.CommerceVisible).HasDefaultValueSql("true");
        entity.HasOne(d => d.Country).WithMany(p => p.Users).HasForeignKey(d => d.CountryId).OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(d => d.Owner).WithMany(p => p.Сustomers).HasForeignKey(d => d.OwnerId).OnDelete(DeleteBehavior.Restrict);
      });

      modelBuilder.Entity<InviteRegistration>(entity =>
      {
        entity.HasOne(d => d.User).WithMany(p => p.InviteRegistrations).HasForeignKey(d => d.OwnerId).OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<Country>(entity =>
      {
        entity.HasData(DatabaseDictionaries.CountriesDic);
      });
      
      modelBuilder.Entity<DevStatus>(entity =>
      {
        entity.HasIndex(e => e.DeviceId);
        entity.HasIndex(e => e.MessageDate);
        entity.HasOne(d => d.Device).WithMany(p => p.DevStatuses).HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<DevErrorStatus>(entity =>
      {
        entity.HasIndex(e => e.DeviceId);
        entity.HasIndex(e => e.MessageDate);
        entity.HasOne(d => d.Device).WithMany(p => p.DevErrorStatuses).HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<DevSale>(entity =>
      {
        entity.HasIndex(e => e.DeviceId);
        entity.HasIndex(e => e.MessageDate);
        entity.HasOne(d => d.Device).WithMany(p => p.DevSales).HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<DevEncash>(entity =>
      {
        entity.HasIndex(e => e.DeviceId);
        entity.HasIndex(e => e.MessageDate);
        entity.HasOne(d => d.Device).WithMany(p => p.DevEncashes).HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<DevAlert>(entity =>
      {
        entity.HasIndex(e => e.DeviceId);
        entity.HasIndex(e => e.MessageDate);
        entity.HasOne(d => d.Device).WithMany(p => p.DevAlerts).HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<DevInfo>(entity =>
      {
        entity.HasIndex(e => new {e.DeviceId, e.Name}).IsUnique();
        entity.HasIndex(e => e.DeviceId);
        entity.HasIndex(e => e.MessageDate);
        entity.HasOne(d => d.Device).WithMany(p => p.DevInfos).HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<DevSetting>(entity =>
      {
        entity.HasIndex(e => new {e.DeviceId, e.MessageDate, e.Md5}).IsUnique();
        entity.HasIndex(e => e.DeviceId);
        entity.HasIndex(e => e.TopicType);
        entity.HasIndex(e => e.MessageDate);
        entity.HasOne(d => d.Device).WithMany(p => p.DevSettings).HasForeignKey(d => d.DeviceId).OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<LogUsr>(entity =>
      {
        entity.HasIndex(e => e.LogDate);
      });

      modelBuilder.Entity<FileModel>(entity =>
      {
        entity.HasIndex(e => e.Visible);
      });

      modelBuilder.Entity<AppSettings>(entity =>
      {
        entity.HasData(DatabaseDictionaries.AppSettingsDic);
      });

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
