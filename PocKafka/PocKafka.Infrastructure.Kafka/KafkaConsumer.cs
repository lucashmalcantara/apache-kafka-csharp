using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using PocKafka.Infrastructure.Kafka.Interfaces;
using System;
using System.Threading;

namespace PocKafka.Infrastructure.Kafka
{
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly SchemaRegistryConfig _schemaRegistryConfig;
        private readonly ConsumerConfig _consumerConfig;

        public KafkaConsumer(
            ConsumerConfig consumerConfig,
            SchemaRegistryConfig schemaRegistryConfig,
            ILogger<KafkaConsumer> logger)
        {
            _logger = logger;
            _consumerConfig = consumerConfig;
            _schemaRegistryConfig = schemaRegistryConfig;
        }

        public void Consume<TKey, TValue>(
            string topic,
            Action<ConsumeResult<TKey, TValue>> action,
            Action<IConsumer<TKey, TValue>, Error> errorHandler = default,
            CancellationToken cancellationToken = default)
        {
            if (errorHandler == null)
                errorHandler = (_, e) => _logger.LogError($"Error: {e.Reason}");

            using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
            using (var consumer =
            new ConsumerBuilder<TKey, TValue>(_consumerConfig)
                .SetKeyDeserializer(new AvroDeserializer<TKey>(schemaRegistry).AsSyncOverAsync())
                .SetValueDeserializer(new AvroDeserializer<TValue>(schemaRegistry).AsSyncOverAsync())
                .SetErrorHandler(errorHandler)
                .Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var cr = consumer.Consume(cancellationToken);

                        action(cr);

                        consumer.Commit(cr);
                    }
                }
                finally
                {
                    consumer.Close();
                }
            }

        }
    }
}
