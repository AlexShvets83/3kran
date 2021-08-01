using CommonVending;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using CommonVending.DbProvider;

namespace ThirdVendingWebApi
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      AppDomain.CurrentDomain.UnhandledException += (sender, arg) => { Console.WriteLine($"{sender} - {arg.ExceptionObject}"); };

      //CreateHostBuilder(args).Build().Run();
      await MqttBroker.MqttInit();

      var host = CreateHostBuilder(args).Build();
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        try
        {
          var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
          var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
          await RoleInitializer.InitializeAsync(userManager, rolesManager);
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred while seeding the database.");
        }
      }

      var settings = AppSettingsDbProvider.GetAppSettings();
      ApplicationSettings.Set(settings);

      //var initDev = new InitDevice();

      await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
      .ConfigureWebHostDefaults(webBuilder =>
      {
//#if RELEASE
        //webBuilder.UseKestrel();
//#endif

        webBuilder.ConfigureKestrel((context, options) =>
        {
          options.Limits.MaxRequestBodySize = int.MaxValue;
        });
        webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
        webBuilder.UseIISIntegration();
        webBuilder.UseStartup<Startup>();
      })
      .ConfigureAppConfiguration((hostingContext, config) =>
      {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile(MainSettings.JsonPath, false, false);
      });
  }
}
