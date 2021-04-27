using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ThirdVendingWebApi
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      AppDomain.CurrentDomain.UnhandledException += (sender, arg) => { Console.WriteLine($"{sender} - {arg.ExceptionObject}"); };

      //CreateHostBuilder(args).Build().Run();
      await MqttInit();

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

      //var initDev = new InitDevice();

      await host.RunAsync();
    }

    private static async Task MqttInit()
    {
      try
      {
        var optionsBuilder = new MqttServerOptionsBuilder().WithDefaultEndpoint()
          .WithDefaultEndpointPort(1883)
          .WithConnectionValidator(c =>
          {
            c.ReasonCode = MqttConnectReasonCode.Success;
            Console.WriteLine(1883);
            LogMessage(c, false);
          })
          .WithSubscriptionInterceptor(c =>
          {
            c.AcceptSubscription = true;
            LogMessage(c, true);
          })
          .WithApplicationMessageInterceptor(c =>
          {
            c.AcceptPublish = true;
            LogMessage(c);
          })
          .WithDisconnectedInterceptor(c =>
          {
            if (c == null) return;

            Console.WriteLine("Disconnect: ClientId = {0}, DisconnectType = {1}}", c.ClientId, c.DisconnectType);
          })
          .WithClientMessageQueueInterceptor(c =>
          {
            if (c == null) return;

            c.AcceptEnqueue = true;
            Console.WriteLine("Queue: ReceiverClientId = {0}, SenderClientId = {1}, Topic = {2}, Payload = {3}", c.ReceiverClientId, c.SenderClientId, c.ApplicationMessage.Topic,
                              c.ApplicationMessage.Payload);
          });

        var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var certificate = new X509Certificate2(Path.Combine(currentPath ?? string.Empty, "certificate.pfx"), "qwerty", X509KeyStorageFlags.Exportable);

        var optionsBuilder8 = new MqttServerOptionsBuilder().WithDefaultEndpoint()
          .WithConnectionBacklog(2000)
          .WithoutDefaultEndpoint() // This call disables the default unencrypted endpoint on port 1883
          .WithEncryptedEndpoint()
          .WithEncryptedEndpointPort(8883)
          .WithEncryptionCertificate(certificate.Export(X509ContentType.Pfx))
          .WithEncryptionSslProtocol(SslProtocols.Tls12)

          //.WithDefaultEndpointPort(8883)
          .WithConnectionValidator(c =>
          {
            c.ReasonCode = MqttConnectReasonCode.Success;
            Console.WriteLine(8883);
            LogMessage(c, false);
          })
          .WithSubscriptionInterceptor(c =>
          {
            c.AcceptSubscription = true;
            LogMessage(c, true);
          })
          .WithApplicationMessageInterceptor(c =>
          {
            c.AcceptPublish = true;
            LogMessage(c);
          })
          .WithDisconnectedInterceptor(c =>
          {
            if (c == null) return;

            Console.WriteLine("Disconnect: ClientId = {0}, DisconnectType = {1}}", c.ClientId, c.DisconnectType);
          })
          .WithClientMessageQueueInterceptor(c =>
          {
            if (c == null) return;

            c.AcceptEnqueue = true;
            Console.WriteLine("Queue: ReceiverClientId = {0}, SenderClientId = {1}, Topic = {2}, Payload = {3}", c.ReceiverClientId, c.SenderClientId, c.ApplicationMessage.Topic,
                              c.ApplicationMessage.Payload);
          });

        var mqttServer = new MqttFactory().CreateMqttServer();
        await mqttServer.StartAsync(optionsBuilder.Build());

        var mqttServer8 = new MqttFactory().CreateMqttServer();
        await mqttServer8.StartAsync(optionsBuilder8.Build());
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    /// <summary>
    ///   Logs the message from the MQTT subscription interceptor context.
    /// </summary>
    /// <param name = "context">The MQTT subscription interceptor context.</param>
    /// <param name = "successful">A <see cref = "bool" /> value indicating whether the subscription was successful or not.</param>
    private static void LogMessage(MqttSubscriptionInterceptorContext context, bool successful)
    {
      if (context == null) { return; }

      var message = successful
                      ? $"New subscription: ClientId = {context.ClientId}, TopicFilter = {context.TopicFilter}"
                      : $"Subscription failed for clientId = {context.ClientId}, TopicFilter = {context.TopicFilter}";
      Console.WriteLine(message);
    }

    /// <summary>
    ///   Logs the message from the MQTT message interceptor context.
    /// </summary>
    /// <param name = "context">The MQTT message interceptor context.</param>
    private static void LogMessage(MqttApplicationMessageInterceptorContext context)
    {
      if (context == null) { return; }

      var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

      Console.WriteLine("Message: ClientId = {0}, Topic = {1}, Payload = {2}, QoS = {3}, Retain-Flag = {4}", context.ClientId, context.ApplicationMessage?.Topic, payload,
                        context.ApplicationMessage?.QualityOfServiceLevel, context.ApplicationMessage?.Retain);
    }

    /// <summary>
    ///   Logs the message from the MQTT connection validation context.
    /// </summary>
    /// <param name = "context">The MQTT connection validation context.</param>
    /// <param name = "showPassword">
    ///   A <see cref = "bool" /> value indicating whether the password is written to the log or
    ///   not.
    /// </param>
    private static void LogMessage(MqttConnectionValidatorContext context, bool showPassword)
    {
      if (context == null) { return; }

      var str = new StringBuilder();
      foreach (var pr in context.GetType().GetProperties()) { str.AppendLine($"{pr.Name} = {pr.GetValue(context, null)}"); }

      Console.WriteLine(str.ToString());

      if (showPassword)
      {
        Console.WriteLine("New connection: ClientId = {0}, Endpoint = {1}, Username = {2}, Password = {3}, CleanSession = {4}", context.ClientId, context.Endpoint,
                          context.Username, context.Password, context.CleanSession);
      }
      else
      {
        Console.WriteLine("New connection: ClientId = {0}, Endpoint = {1}, Username = {2}, CleanSession = {3}", context.ClientId, context.Endpoint, context.Username,
                          context.CleanSession);
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
      .ConfigureWebHostDefaults(webBuilder =>
      {
#if RELEASE
          webBuilder.UseKestrel();
#endif
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
