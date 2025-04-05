using Confluent.Kafka;
using System.Text.Json;
namespace b3;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IProducer<string, string> _producer;
    private readonly string _topic = "test-topic";
    private decimal _currentPrice = 175.50m;
    private readonly Random _random = new Random();

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        
        var config = new ProducerConfig
        {
            BootstrapServers = "kafka:29092",
            ClientId = "b3-worker"
        };
        
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var timestamp = DateTime.UtcNow;
                var previousPrice = _currentPrice;
                var priceChange = Math.Round((decimal)(_random.NextDouble() * 2 - 1), 2); // Random change between -1 and 1
                _currentPrice = Math.Round(previousPrice + priceChange, 2);
                var priceChangePercentage = Math.Round((priceChange / previousPrice) * 100, 2);

                var message = new {
                    AssetId = 1,
                    AssetType = "VariableIncome",
                    Symbol = "AAPL",
                    Name = "Apple Inc.",
                    CurrentPrice = _currentPrice,
                    PreviousPrice = previousPrice,
                    PriceChange = priceChange,
                    PriceChangePercentage = priceChangePercentage,
                    Volume = _random.Next(1000000, 10000000),
                    Timestamp = timestamp,
                    Market = "NASDAQ"
                };

                var messageJson = JsonSerializer.Serialize(message);
                _logger.LogInformation("Sending message: {Message}", messageJson);
                
                try
                {
                    await _producer.ProduceAsync(_topic, new Message<string, string>
                    {
                        Key = Guid.NewGuid().ToString(),
                        Value = messageJson
                    }, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error producing message to Kafka");
                }
            }
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _producer?.Dispose();
        base.Dispose();
    }
}
