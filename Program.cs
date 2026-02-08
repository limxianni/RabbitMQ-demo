using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQDemo.Shared;
using System.Text;
using System.Text.Json;

// Demo configuration - change for your RabbitMQ server
const string QueueName = "demo-queue";
var factory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

switch (mode)
{
    case "producer":
    case "send":
        await RunProducerAsync(factory, QueueName);
        break;
    case "consumer":
    case "receive":
        await RunConsumerAsync(factory, QueueName);
        break;
    default:
        PrintUsage(); // Cannot have producer and consumer when running this line
        break;
}

static void PrintUsage()
{
    Console.WriteLine("RabbitMQ .NET 10 Demo");
    Console.WriteLine("=====================");
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run producer   Send sample messages to the queue");
    Console.WriteLine("  dotnet run consumer  Receive and display messages (Ctrl+C to stop)");
    Console.WriteLine();
    Console.WriteLine("Ensure RabbitMQ is running (e.g. docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:management)");
}

static async Task RunProducerAsync(ConnectionFactory factory, string queueName)
{
    Console.WriteLine("Producer: Connecting to RabbitMQ...");

    using var connection = await factory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync();

    await channel.QueueDeclareAsync(
        queue: queueName,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    Console.WriteLine($"Producer: Sending messages to queue '{queueName}'...");

    var messages = new MessageModel[]
    {
        new MessageModel { Id = 1, Text = "First message", Timestamp = DateTime.UtcNow },
        new MessageModel { Id = 2, Text = "Second message", Timestamp = DateTime.UtcNow },
        new MessageModel { Id = 3, Text = "Third message", Timestamp = DateTime.UtcNow }
    };

    foreach (var msg in messages)
    {
        var json = JsonSerializer.Serialize(msg);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            body: body);

        Console.WriteLine($"  Sent: {msg.Text} (Id: {msg.Id})");
    }

    Console.WriteLine("Producer: Done sent. Messages are in the queue.");
}

static async Task RunConsumerAsync(ConnectionFactory factory, string queueName)
{
    Console.WriteLine("Consumer: Connecting to RabbitMQ...");

    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    await channel.QueueDeclareAsync(
        queue: queueName,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    Console.WriteLine($"Consumer: Waiting for messages on queue '{queueName}'... (Ctrl+C to exit)");

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += (_, ea) =>
    {
        var body = ea.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);
        Console.WriteLine($"  Received: {json}");
        return Task.CompletedTask;
    };

    await channel.BasicConsumeAsync(
        queue: queueName,
        autoAck: true,
        consumer: consumer);

    var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
    try
    {
        await Task.Delay(Timeout.Infinite, cts.Token).ConfigureAwait(false);
    }
    catch (OperationCanceledException) { throw new Exception("Consumer cancelled"); }
}
