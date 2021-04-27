using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;

namespace MqttTest
{
  class Program
  {
    static async Task Main(string[] args)
    {
      Console.WriteLine("Hello World!");

      var message = new MqttApplicationMessageBuilder()
        .WithTopic("MyTopic")
        .WithPayload("Hello World")
        .WithExactlyOnceQoS()
        .WithRetainFlag()
        .Build();
      //// Use WebSocket connection.
      //var options = new MqttClientOptionsBuilder()
      //  .WithWebSocketServer("broker.hivemq.com:8000/mqtt")
      //  .Build();

      // Use TCP connection.
      var options = new MqttClientOptionsBuilder()
        .WithTcpServer("95.183.10.198", 1883) // Port is optional
        .Build();
      try
      {
        // Create a new MQTT client.
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();
        await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
        await mqttClient.PublishAsync(message, CancellationToken.None);
        Console.WriteLine("Message send!");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      Console.ReadLine();
    }
  }
}
