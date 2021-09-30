using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using PocKafka.Infrastructure.Kafka.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PocKafka.Infrastructure.Kafka
{
    public class KafkaPublisher<TKey, TValue> : IKafkaPublisher<TKey, TValue>
    {
        private readonly IProducer<TKey, TValue> _producer;
        private readonly ILogger<KafkaPublisher<TKey, TValue>> _logger;

        public KafkaPublisher(
            ProducerConfig producerConfig,
            SchemaRegistryConfig schemaRegistryConfig,
            ILogger<KafkaPublisher<TKey, TValue>> logger)
        {
            _logger = logger;
            _producer = new ProducerBuilder<TKey, TValue>(producerConfig)
                .SetKeySerializer(new AvroSerializer<TKey>(new CachedSchemaRegistryClient(schemaRegistryConfig)).AsSyncOverAsync())
                .SetValueSerializer(new AvroSerializer<TValue>(new CachedSchemaRegistryClient(schemaRegistryConfig)).AsSyncOverAsync())
                .SetErrorHandler((_, e) => _logger.LogError(e.Reason))
                .Build();
        }

        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, TKey key, TValue value, CancellationToken cancellationToken)
        {
            var message = new Message<TKey, TValue>()
            {
                Key = key,
                Value = value
            };

            var deliveryResult = await _producer.ProduceAsync(topic, message, cancellationToken);

            return deliveryResult;
        }
    }
}
