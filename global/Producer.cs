using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace global;

public class Producer : IDisposable
{
    private readonly ILogger<Producer> _logger;
    private readonly IProducer<string, string> _producer;
    private readonly string _topic = "lemon-bank-variable-income-topic";
    private readonly string _bootstrapServers;

    public Producer(ILogger<Producer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:29092";
        
        var config = new ProducerConfig
        {
            BootstrapServers = _bootstrapServers,
            ClientId = "global-producer"
        };
        
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync(string key, string value)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = key,
                Value = value
            };

            var deliveryResult = await _producer.ProduceAsync(_topic, message);
            _logger.LogInformation("Message delivered to {Topic} [{Partition}] @ {Offset}", 
                deliveryResult.Topic, 
                deliveryResult.Partition, 
                deliveryResult.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Error producing message to Kafka");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
} 