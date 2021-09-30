using Confluent.Kafka;
using System.Threading;
using System.Threading.Tasks;

namespace PocKafka.Infrastructure.Kafka.Interfaces
{
    public interface IKafkaPublisher<TKey, TValue>
    {
        Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, TKey key, TValue value, CancellationToken cancellationToken);
    }
}
