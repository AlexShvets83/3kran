﻿using CommonVending.Crypt;
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
    private static readonly string Host = MainSettings.Settings.MainHost;

    public static async Task MqttInit()
    {
      try
      {
        await MqttServerAesInit();

        var pathCert = $"/etc/letsencrypt/live/{Host}/fullchain.pem";
        var pathKey = $"/etc/letsencrypt/live/{Host}/privkey.pem";
        var fc = await File.ReadAllTextAsync(pathCert);
        var fk = await File.ReadAllTextAsync(pathKey);
        var certificate = X509Certificate2.CreateFromPem(fc, fk);

        var optionsBuilder = new MqttServerOptionsBuilder()
          .WithConnectionBacklog(20000)
          .WithClientId(Host)
          .WithoutDefaultEndpoint() // This call disables the default unencrypted endpoint on port 1883
          .WithEncryptedEndpoint()
          .WithEncryptedEndpointPort(8883)
          .WithEncryptionCertificate(certificate.Export(X509ContentType.Pfx))
          //.WithRemoteCertificateValidationCallback(ClientCertificateValidationCallback)
          //.WithClientCertificate(ClientCertificateValidationCallback)
          //.WithEncryptionSslProtocol(SslProtocols.Tls | SslProtocols.Tls11| SslProtocols.Tls12 | SslProtocols.Tls13)
          .WithEncryptionSslProtocol(SslProtocols.None)
          .WithConnectionValidator(ConnectionValidatorCallback)
          .WithSubscriptionInterceptor(c => { c.AcceptSubscription = true; })
          .WithApplicationMessageInterceptor(async c =>
          {
            c.AcceptPublish = true;
            await LogMessage(c, 8883);
          });

        var mqttServer = new MqttFactory().CreateMqttServer();
        await mqttServer.StartAsync(optionsBuilder.Build());
        Console.WriteLine($"MQTT host: {Host}");
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    private static async Task MqttServerAesInit()
    {
      var optionsBuilder = new MqttServerOptionsBuilder()
        .WithConnectionBacklog(20000)
        .WithClientId(Host)
        .WithDefaultEndpoint()
        .WithDefaultEndpointPort(1883)
        .WithConnectionValidator(ConnectionValidatorCallbackAes)
        .WithSubscriptionInterceptor(c =>
        {
          c.AcceptSubscription = true;

          //LogSubscription(c, true);
        })
        .WithApplicationMessageInterceptor(async c =>
        {
          c.AcceptPublish = true;
          await LogMessage(c, 1883);
        });
      var mqttServer = new MqttFactory().CreateMqttServer();
      await mqttServer.StartAsync(optionsBuilder.Build());
    }

    private static void ConnectionValidatorCallbackAes(MqttConnectionValidatorContext c)
    {
      if (c == null) return;
      
      if (!ApplicationSettings.SupportBoard3)
      {
        Console.WriteLine("Port: 1883 - Not supported this board [3]!");
        c.ReasonCode = MqttConnectReasonCode.RetainNotSupported;
        return;
      }

      if (!c.ClientId.Contains("device_"))
      {
        Console.WriteLine($"Port: 1883 - Bad username or password for client {c.ClientId}!");
        c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        return;
      }

      Console.WriteLine("Port: 1883 - New connection: ClientId = {0}, Endpoint = {1}, Username = {2}, CleanSession = {3}", c.ClientId, c.Endpoint, c.Username, c.CleanSession);
      c.ReasonCode = MqttConnectReasonCode.Success;
    }

    private static void ConnectionValidatorCallback(MqttConnectionValidatorContext c)
    {
      if (c == null) return;

      if (!ApplicationSettings.SupportBoard2)
      {
        Console.WriteLine("Port: 8883 - Not supported this board!");
        c.ReasonCode = MqttConnectReasonCode.RetainNotSupported;
        return;
      }

      if (c.Username != "3voda" || c.Password != "Leimnoj8Knod")
      { 
        Console.WriteLine($"Port: 8883 - Bad username or password for client {c.ClientId}!");
        c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        return;
      }

      if (c.Password != "Leimnoj8Knod")
      {
        Console.WriteLine($"Port: 8883 - Bad username or password for client {c.ClientId}!");
        c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
        return;
      }

      Console.WriteLine("Port: 8883 - New connection: ClientId = {0}, Endpoint = {1}, Username = {2}, CleanSession = {3}", c.ClientId, c.Endpoint, c.Username, c.CleanSession);
      //Console.WriteLine($"New connection: {c.ClientId}");
      c.ReasonCode = MqttConnectReasonCode.Success;
    }

    /// <summary>
    ///   Logs the message from the MQTT message interceptor context.
    /// </summary>
    /// <param name = "context">The MQTT message interceptor context.</param>
    /// <param name="port"></param>
    private static async Task LogMessage(MqttApplicationMessageInterceptorContext context, int port)
    {
      if (context?.ApplicationMessage == null) return;

      var type = 2;
      var topic = context.ApplicationMessage.Topic;
      var payloadBytes = context.ApplicationMessage.Payload ?? Array.Empty<byte>();
      var payload = Encoding.UTF8.GetString(payloadBytes);
      if (!payload.StartsWith("{"))
      {
        var hexEnc = new StringBuilder(context.ApplicationMessage.Payload.Length * 2);
        foreach (var b in payloadBytes) { hexEnc.AppendFormat("{0:x2} ", b); }

        var plainText = CryptoAes.DecryptAes(payloadBytes);
        payload = plainText;
        type = 3;
        //Console.WriteLine("Port: {0} Type: [{1}] Topic = {2}, Payload = {3}, Qos = {4}", port, type, topic, payload.Replace("\0", string.Empty).Trim(), context.ApplicationMessage.QualityOfServiceLevel);
      }

      payload = payload.Replace("\0", string.Empty);
      payload = payload.Trim();

      Console.WriteLine("Port: {0} Type: [{1}] Topic = {2}, Payload = {3}, Qos = {4}", port, type, topic, payload.Replace("\0", string.Empty).Trim(), context.ApplicationMessage.QualityOfServiceLevel);
      await DeviceMqtt.MessageHandler(topic, payload, context);

      //Console.WriteLine("Topic = {0}, Payload = {1}", context.ApplicationMessage?.Topic, payload);
      //Console.WriteLine("Message: ClientId = {0}, Topic = {1}, Payload = {2}, QoS = {3}, Retain-Flag = {4}",
      //context.ClientId, context.ApplicationMessage?.Topic, payload,
      //context.ApplicationMessage?.QualityOfServiceLevel, context.ApplicationMessage?.Retain);
    }
    
    ///// <summary>
    /////   Logs the message from the MQTT subscription interceptor context.
    ///// </summary>
    ///// <param name = "context">The MQTT subscription interceptor context.</param>
    ///// <param name = "successful">A <see cref = "bool" /> value indicating whether the subscription was successful or not.</param>
    //private static void LogSubscription(MqttSubscriptionInterceptorContext context, bool successful)
    //{
    //  if (context == null) { return; }

    //  var message = successful
    //                  ? $"New subscription: ClientId = {context.ClientId}, TopicFilter = {context.TopicFilter}"
    //                  : $"Subscription failed for clientId = {context.ClientId}, TopicFilter = {context.TopicFilter}";
    //  Console.WriteLine(message);
    //}

    //private static bool ClientCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
    //{
    //    if (certificate != null) { Console.WriteLine("certificate Issuer = {0}", certificate.Issuer); }

    //    if (chain != null) { Console.WriteLine("chain ChainPolicy = {0}", chain.ChainPolicy); }

    //    Console.WriteLine("SslPolicyErrors = {0}", sslpolicyerrors);
    //    return true;
    //}
  }
}
