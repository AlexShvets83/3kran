using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace CommonVending
{
  public class MqttBroker
  {
    public static async Task MqttInit()
    {
      try
      {
        //var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //chain   -- errror
        //cert
        //fullchain
        var pathCert = "/etc/letsencrypt/live/monitoring3voda.ru/fullchain.pem";
        var pathKey = "/etc/letsencrypt/live/monitoring3voda.ru/privkey.pem";

#if DEBUG
        pathCert = "d:\\!Projects\\!3Cran\\SSL\\monitoring3voda.ru\\cert.pem";
        pathKey = "d:\\!Projects\\!3Cran\\SSL\\monitoring3voda.ru\\privkey.pem";
#endif
        var fc = await File.ReadAllTextAsync(pathCert);
        var fk = await File.ReadAllTextAsync(pathKey);

        var certificate = X509Certificate2.CreateFromPem(fc, fk);

        //var fc = await File.ReadAllTextAsync("/etc/letsencrypt/live/monitoring3voda.ru/mqtt.csr");
        //var fk = await File.ReadAllTextAsync("/etc/letsencrypt/live/monitoring3voda.ru/mqtt.key");
        //var certificate = X509Certificate2.CreateFromPem(fc, fk);

        //var currentPath = "/etc/letsencrypt/live/monitoring3voda.ru/";
        //var certificate = new X509Certificate2(Path.Combine(currentPath ?? string.Empty, "certificate.pfx"), "qwerty", X509KeyStorageFlags.Exportable);

        //var certifOption = new MqttServerOptions();
        //certifOption.
        //certifOption.TlsEndpointOptions.Certificate = certificate.Export(X509ContentType.Pfx);
        //certifOption.TlsEndpointOptions.IsEnabled = true;

        var optionsBuilder = new MqttServerOptionsBuilder().WithConnectionBacklog(2000)
          .WithClientId("monitoring3voda.ru")
          ////.WithoutDefaultEndpoint() // This call disables the default unencrypted endpoint on port 1883
          .WithEncryptedEndpoint()

          //.WithDefaultCommunicationTimeout(TimeSpan.FromMinutes(180))
          //.WithMultiThreadedApplicationMessageInterceptor()
          .WithEncryptedEndpointPort(8883)
          .WithEncryptionCertificate(certificate.Export(X509ContentType.Pfx))
          .WithClientCertificate(ValidationCallback)

          //.WithRemoteCertificateValidationCallback(ValidationCallback)
          //.WithPersistentSessions()
          .WithEncryptionSslProtocol(SslProtocols.None)

          //.WithDefaultEndpointPort(8883)
          .WithConnectionValidator(ConnectionValidatorCallback)
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
            Console.WriteLine("Disconnect: ClientId = {0}, DisconnectType = {1}}", c.ClientId, c.DisconnectType);
          });
          //.WithClientMessageQueueInterceptor(c =>
          //{
          //  c.AcceptEnqueue = true;
          //  var payload = c.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(c.ApplicationMessage?.Payload);
          //  Console.WriteLine("Queue: ReceiverClientId = {0}, SenderClientId = {1}, Topic = {2}, Payload = {3}", c.ReceiverClientId, c.SenderClientId, c.ApplicationMessage.Topic,
          //                    payload);
          //})
          //.WithMultiThreadedApplicationMessageInterceptor(c =>
          //{
          //  c.AcceptPublish = true;
          //  LogMessage(c);
          //});

        var mqttServer = new MqttFactory().CreateMqttServer();
        await mqttServer.StartAsync(optionsBuilder.Build());
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    private static void ConnectionValidatorCallback(MqttConnectionValidatorContext c)
    {
      if (c == null) return;

      if (c.Username != "3voda")
      {
        c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        return;
      }

      if (c.Password != "Leimnoj8Knod")
      {
        c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        return;
      }

      c.ReasonCode = MqttConnectReasonCode.Success;
      Console.WriteLine("New connection: ClientId = {0}, Endpoint = {1}, Username = {2}, CleanSession = {3}", c.ClientId, c.Endpoint, c.Username, c.CleanSession);
    }

    private static bool ValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslpolicyerrors)
    {
      if (certificate != null) { Console.WriteLine("certificate Issuer = {0}", certificate.Issuer); }

      if (chain != null) { Console.WriteLine("chain ChainPolicy = {0}", chain.ChainPolicy); }

      return true;
    }

    /// <summary>
    ///   Logs the message from the MQTT subscription interceptor context.
    /// </summary>
    /// <param name = "context">The MQTT subscription interceptor context.</param>
    /// <param name = "successful">A <see cref = "bool" /> value indicating whether the subscription was successful or not.</param>
    private static void LogMessage(MqttSubscriptionInterceptorContext context, bool successful)
    {
      if (context == null) { return; }

      //context.SessionItems.Add();
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

      DeviceMqtt.MessageHandler(context.ApplicationMessage?.Topic, payload);
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

      Console.WriteLine("New connection: ClientId = {0}, Endpoint = {1}, Username = {2}, CleanSession = {3}", context.ClientId, context.Endpoint, context.Username,
                        context.CleanSession);

      //var str = new StringBuilder();
      //foreach (var pr in context.GetType().GetProperties()) { str.AppendLine($"{pr.Name} = {pr.GetValue(context, null)}"); }

      //Console.WriteLine(str.ToString());
      //if (showPassword)
      //{
      //  Console.WriteLine("New connection: ClientId = {0}, Endpoint = {1}, Username = {2}, Password = {3}, CleanSession = {4}", context.ClientId, context.Endpoint,
      //                    context.Username, context.Password, context.CleanSession);
      //}
      //else
      //{
      //  Console.WriteLine("New connection: ClientId = {0}, Endpoint = {1}, Username = {2}, CleanSession = {3}", context.ClientId, context.Endpoint, context.Username,
      //                    context.CleanSession);
      //}
    }
  }
}
