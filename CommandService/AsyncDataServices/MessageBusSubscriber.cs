using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IModel _channel;
    private QueueDeclareOk _queue;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
        
        Initialize();
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (moduleHandle, e) =>
        {
            Console.WriteLine("--> Event Received!");

            ReadOnlyMemory<byte> body = e.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            
            _eventProcessor.ProcessEvent(notificationMessage);
        };

        _channel.BasicConsume(_queue.QueueName, true, consumer);
        
        return Task.CompletedTask;
    }

    private void Initialize()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"])
        };
        
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queue = _channel.QueueDeclare();
            _channel.QueueBind(_queue.QueueName, "trigger", "");
            
            Console.WriteLine("--> Listening on the MessageBus");
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not connect to the MessageBus: {e.Message}");
        }
    }
    
    public override void Dispose()
    {
        Console.WriteLine("MessageBus Disposed");
        if (!_channel.IsOpen) return;
        _channel.Close();
        _connection.Close();
    }
}