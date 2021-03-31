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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { optionsBuilder.UseSnakeCaseNamingConvention(); }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
