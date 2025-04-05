using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace global;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly string _topic = "test-topic";
    private readonly string _bootstrapServers;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:29092";
        
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "global-worker-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    
                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation("Received message: {Message}", consumeResult.Message.Value);
                        
                        // Process the message here
                        // For example, you could deserialize the JSON message
                        // var message = JsonSerializer.Deserialize<YourMessageType>(consumeResult.Message.Value);
                        
                        // Commit the offset after successful processing
                        _consumer.Commit(consumeResult);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer operation was cancelled");
        }
        finally
        {
            _consumer.Close();
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
