using Confluent.Kafka;
using System;
using System.Threading;

namespace PocKafka.Infrastructure.Kafka.Interfaces
{
    public interface IKafkaConsumer
    {
        void Consume<TKey, TValue>(string topic,
            Action<ConsumeResult<TKey, TValue>> action,
            Action<IConsumer<TKey, TValue>, Error> errorHandler = default,
            CancellationToken cancellationToken = default);
    }
}
