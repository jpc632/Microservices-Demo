using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQHost"],
            Port = int.Parse(configuration["RabbitMQPort"])
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            
            Console.WriteLine("--> Connected to the MessageBus");
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not connect to the MessageBus: {e.Message}");
        }
    }
    
    public void PublishNewPlatform(PlatformPublished platformPublished)
    {
        var message = JsonSerializer.Serialize(platformPublished);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Connection Open, Sending Message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ Connection Closed, not sending");
        }
    }

    public void Dispose()
    {
        Console.WriteLine("MessageBus Disposed");
        if (!_channel.IsOpen) return;
        _channel.Close();
        _connection.Close();
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        _channel.BasicPublish(
            "trigger", 
            "",
            null,
            body);
        
        Console.WriteLine($"--> Message sent: {message}");
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }
}