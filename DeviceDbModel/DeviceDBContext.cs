using DeviceDbModel.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { optionsBuilder.UseSnakeCaseNamingConvention(); }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Device>(entity =>
      {
        entity.HasIndex(e => e.DeviceId).IsUnique();
        entity.HasIndex(e => e.Address);
        entity.HasOne(d => d.User).WithMany(p => p.Devices).HasForeignKey(d => d.OwnerId).OnDelete(DeleteBehavior.Restrict);
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
        entity.HasOne(d => d.Country).WithMany(p => p.Users).HasForeignKey(d => d.CountryId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(d => d.Master).WithMany(p => p.Slaves).HasForeignKey(d => d.OwnerId).OnDelete(DeleteBehavior.Restrict);
      });

      modelBuilder.Entity<InviteRegistration>(entity =>
      {
        entity.HasOne(d => d.User).WithMany(p => p.InviteRegistrations).HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
      });

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
