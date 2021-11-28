using CommonVending.Crypt;
using DeviceDbModel;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CommonVending
{
  public static class MqttBroker
  {
    public static async Task MqttInit()
    {
      try
      {
        await MqttServerAesInit();

        //var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //chain   -- errror
        //cert
        //fullchain
        var host = MainSettings.Settings.MainHost;
        var pathCert = $"/etc/letsencrypt/live/{host}/fullchain.pem";
        var pathKey = $"/etc/letsencrypt/live/{host}/privkey.pem";

        var fc = await File.ReadAllTextAsync(pathCert);
        var fk = await File.ReadAllTextAsync(pathKey);

        var certificate = X509Certificate2.CreateFromPem(fc, fk);
        var optionsBuilder = new MqttServerOptionsBuilder().WithConnectionBacklog(20000)
            .WithClientId(host)
            .WithoutDefaultEndpoint() // This call disables the default unencrypted endpoint on port 1883
            .WithEncryptedEndpoint()

            //.WithDefaultCommunicationTimeout(TimeSpan.FromMinutes(180))
            //.WithMultiThreadedApplicationMessageInterceptor()
            .WithEncryptedEndpointPort(8883)
            .WithEncryptionCertificate(certificate.Export(X509ContentType.Pfx))

            //.WithClientCertificate(ClientCertificateValidationCallback)
            .WithEncryptionSslProtocol(SslProtocols.None)
            .WithConnectionValidator(ConnectionValidatorCallback)
            .WithSubscriptionInterceptor(c => { c.AcceptSubscription = true; })
            .WithApplicationMessageInterceptor(async c =>
            {
              c.AcceptPublish = true;
              await LogMessage(c);
            });

        var mqttServer = new MqttFactory().CreateMqttServer();
        await mqttServer.StartAsync(optionsBuilder.Build());
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    private static async Task MqttServerAesInit()
    {
      var optionsBuilder = new MqttServerOptionsBuilder().WithConnectionBacklog(2000)
          .WithClientId("monitoring3voda.ru")
          .WithDefaultEndpoint()
          .WithDefaultEndpointPort(1883)
          .WithConnectionValidator(ConnectionValidatorCallbackAes)
          .WithSubscriptionInterceptor(c => { c.AcceptSubscription = true; })
          .WithApplicationMessageInterceptor(async c =>
          {
            c.AcceptPublish = true;
            await LogMessage(c);
          });
      var mqttServer = new MqttFactory().CreateMqttServer();
      await mqttServer.StartAsync(optionsBuilder.Build());
    }

    private static void ConnectionValidatorCallbackAes(MqttConnectionValidatorContext c)
    {
      if (c == null) return;
      
      if (!ApplicationSettings.SupportBoard3)
      {
        c.ReasonCode = MqttConnectReasonCode.RetainNotSupported;
        return;
      }

      if (!c.ClientId.Contains("device_"))
      {
        c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        return;
      }

      c.ReasonCode = MqttConnectReasonCode.Success;
    }

    private static void ConnectionValidatorCallback(MqttConnectionValidatorContext c)
    {
      if (c == null) return;
      
      if (!ApplicationSettings.SupportBoard2)
      {
        c.ReasonCode = MqttConnectReasonCode.RetainNotSupported;
        return;
      }

      //Console.WriteLine("New connection: ClientId = {0}, Endpoint = {1}, Username = {2}, CleanSession = {3}", c.ClientId, c.Endpoint, c.Username, c.CleanSession);

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

      //Console.WriteLine($"New connection: {c.ClientId}");
      c.ReasonCode = MqttConnectReasonCode.Success;
    }

    //private static bool ClientCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslpolicyerrors)
    //{
    //  if (certificate != null) { Console.WriteLine("certificate Issuer = {0}", certificate.Issuer); }

    //  if (chain != null) { Console.WriteLine("chain ChainPolicy = {0}", chain.ChainPolicy); }

    //  Console.WriteLine("SslPolicyErrors = {0}", sslpolicyerrors);
    //  return true;
    //}

    ///// <summary>
    /////   Logs the message from the MQTT subscription interceptor context.
    ///// </summary>
    ///// <param name = "context">The MQTT subscription interceptor context.</param>
    ///// <param name = "successful">A <see cref = "bool" /> value indicating whether the subscription was successful or not.</param>
    //private static async Task LogSubscription(MqttSubscriptionInterceptorContext context, bool successful)
    //{
    //  if (context == null) { return; }

    //  //await DeviceMqtt.SubscriptionHandler(context.TopicFilter.Topic);
    //  var message = successful
    //                  ? $"New subscription: ClientId = {context.ClientId}, TopicFilter = {context.TopicFilter}"
    //                  : $"Subscription failed for clientId = {context.ClientId}, TopicFilter = {context.TopicFilter}";
    //  Console.WriteLine(message);
    //}

    /// <summary>
    ///   Logs the message from the MQTT message interceptor context.
    /// </summary>
    /// <param name = "context">The MQTT message interceptor context.</param>
    private static async Task LogMessage(MqttApplicationMessageInterceptorContext context)
    {
      if (context == null) { return; }

      //var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);
      var topic = context.ApplicationMessage?.Topic;
      var payloadBytes = context.ApplicationMessage?.Payload ?? Array.Empty<byte>();
      var payload = Encoding.UTF8.GetString(payloadBytes);
      if (!payload.StartsWith("{"))
      {
        var hexEnc = new StringBuilder(context.ApplicationMessage.Payload.Length * 2);
        foreach (var b in payloadBytes) { hexEnc.AppendFormat("{0:x2} ", b); }

        var plainText = CryptoAes.DecryptAes(payloadBytes);
        payload = plainText;
        Console.WriteLine("Topic = {0}, Payload = {1}", topic, payload);
      }

      payload = payload.Replace("\0", string.Empty);
      payload = payload.Trim();

      await DeviceMqtt.MessageHandler(topic, payload);

      //Console.WriteLine("Topic = {0}, Payload = {1}", context.ApplicationMessage?.Topic, payload);

      //Console.WriteLine("Message: ClientId = {0}, Topic = {1}, Payload = {2}, QoS = {3}, Retain-Flag = {4}", context.ClientId, context.ApplicationMessage?.Topic, payload,
      //context.ApplicationMessage?.QualityOfServiceLevel, context.ApplicationMessage?.Retain);
    }
  }
}
