using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;

namespace MqttTest
{
  class Program
  {
    static async Task Main(string[] args)
    {
      Console.WriteLine("Hello World!");
      //869244046767509 - my   @"{""maintain"":0}"
      //869395032100038   - 3kran  "priceCard":1.000
      var topic = "3voda/device/8693950321000381/status";
      //var topic = "3voda/device/869395032100038/cleaner/status";

      var pl = @"{""timestamp"":1623325680,""totalSold"":15,""totalMoney"":160.000,""temperature"":15.280,""status"":0}";
      //var pl = @"{""timestamp"":1623320006,""tds"":25,""status"":4}";
      var message = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(pl).WithExactlyOnceQoS().WithRetainFlag().Build();
      //var message = new MqttApplicationMessageBuilder()
      //  .WithTopic("MyTopic")
      //  .WithPayload("Hello World")
      //  .WithExactlyOnceQoS()
      //  .WithRetainFlag()
      //  .Build();
      //// Use WebSocket connection.
      //var options = new MqttClientOptionsBuilder()
      //  .WithWebSocketServer("broker.hivemq.com:8000/mqtt")
      //  .Build();

      var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      //var currentPath = "/etc/letsencrypt/live/monitoring3voda.ru/";
      var certificate = new X509Certificate2(Path.Combine(currentPath ?? string.Empty, "certificate.pfx"), "qwerty", X509KeyStorageFlags.Exportable);

      List<X509Certificate> certs = new List<X509Certificate>
      {
        certificate
      };

      // Use TCP connection.
      var options = new MqttClientOptionsBuilder()
        //.WithTcpServer("95.183.10.198", 8883) // Port is optional
        .WithTcpServer("192.168.1.250", 8883)
        //.WithTcpServer("monitoring3voda.ru", 8883)
        .WithCredentials("3voda", "Leimnoj8Knod")
        //.WithTls(new MqttClientOptionsBuilderTlsParameters
        //{
        //  UseTls = true,
        //  SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
        //  Certificates = certs
        //})

        //.WithTls(new MqttClientOptionsBuilderTlsParameters
        //{
        //  UseTls = true,
        //  SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
        //  //Certificates = certs
        //})
        .Build();
      try
      {
        // Create a new MQTT client.
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();
        await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
        await mqttClient.PublishAsync(message, CancellationToken.None);
        Console.WriteLine("Message sent!");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      Console.ReadLine();
    }
  }
}
