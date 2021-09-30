using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PocKafka.Infrastructure.Kafka.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PocKafka.Api.HostedServices
{
    public class ConsumerHostedService : BackgroundService
    {
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _kafkaTopic;

        public ConsumerHostedService(
            ILogger<ConsumerHostedService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ConsumerHostedService)} is starting.", null);

            stoppingToken.Register(() =>
                _logger.LogInformation($"{nameof(ConsumerHostedService)} background task is stopping, because a cancellation was requested."));

            Action<ConsumeResult<string, Infrastructure.Kafka.Models.Location>> printMessage = consumeResult =>
            {
                _logger.LogInformation($"A new record has been consumed. Topic: '{consumeResult.Topic}' | Partition: '{consumeResult.Partition}' | Offset: '{consumeResult.Offset}' | Key: '{consumeResult.Message.Key}' | Value: {JsonSerializer.Serialize(consumeResult.Message.Value)}");
            };

            Action<IConsumer<string, Infrastructure.Kafka.Models.Location>, Error> errorHandler = (consumer, error) =>
            {
                _logger.LogInformation($"Error consuming Kafka topic(s) '{string.Join(',', consumer.Subscription)}': {error.Reason}");
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var kafkaConsumer = scope.ServiceProvider.GetRequiredService<IKafkaConsumer>();

                        await Task.Run(() => kafkaConsumer.Consume(_kafkaTopic, printMessage, errorHandler, stoppingToken));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Error '{ex.Message}' consuming topic '{_kafkaTopic}'. Details: {ex}");
                    await DelayToRecover();
                }
            }

            _logger.LogInformation($"{nameof(ConsumerHostedService)} background task is stopping.");
        }

        private Task DelayToRecover() => Task.Delay(millisecondsDelay: 5000);
    }
}
