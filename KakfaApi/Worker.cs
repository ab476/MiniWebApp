using Confluent.Kafka;

namespace KakfaApi;

internal sealed class Worker(IProducer<string, string> producer) : BackgroundService
{
    // Use producer...
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        await producer.ProduceAsync("my-topic", new Message<string, string> { Key = "01", Value = "Hello, Kafka!" }, stoppingToken);

        producer.Flush(stoppingToken);
        producer.Dispose();

    }
}
