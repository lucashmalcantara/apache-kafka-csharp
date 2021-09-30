using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;
using PocKafka.Infrastructure.Kafka.Interfaces;
using System;
using System.Net;

namespace PocKafka.Infrastructure.Kafka
{
    public static class Configure
    {
        public static IServiceCollection AddKafka(this IServiceCollection services)
        {
            var username = Environment.GetEnvironmentVariable("KAFKA_BROKER_USERNAME");
            var password = Environment.GetEnvironmentVariable("KAFKA_BROKER_PASSWORD");
            var bootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS");
            var schemaRegistryUrl = Environment.GetEnvironmentVariable("SCHEMA_REGISTRY_URL");
            var schemaRegistryBasicAuthUserInfo = Environment.GetEnvironmentVariable("SCHEMA_REGISTRY_BASIC_AUTH_USER_INFO");
            var consumerGroupId = Environment.GetEnvironmentVariable("CONSUMER_GROUP_ID");
            var sslCaLocation = Environment.GetEnvironmentVariable("SSL_CA_LOCATION");
            var sslEndpointIdentificationAlgorithm = sslCaLocation == null ? (SslEndpointIdentificationAlgorithm?)null : SslEndpointIdentificationAlgorithm.Https;

            if (username != null && password != null)
            {
                services
                    .AddSingleton(sp => new ProducerConfig
                    {
                        BootstrapServers = bootstrapServers,
                        ClientId = Dns.GetHostName(),
                        CompressionType = CompressionType.Zstd,
                        SecurityProtocol = SecurityProtocol.SaslSsl,
                        SaslMechanism = SaslMechanism.Plain,
                        SaslUsername = username,
                        SaslPassword = password,
                        Acks = Acks.All,
                        EnableIdempotence = true
                    })
                    .AddSingleton(sp => new ConsumerConfig
                    {
                        BootstrapServers = bootstrapServers,
                        ClientId = Dns.GetHostName(),
                        SecurityProtocol = SecurityProtocol.SaslSsl,
                        SaslMechanism = SaslMechanism.Plain,
                        SaslUsername = username,
                        SaslPassword = password,
                        SslEndpointIdentificationAlgorithm = sslEndpointIdentificationAlgorithm,
                        SslCaLocation = sslCaLocation,
                        GroupId = consumerGroupId
                    })
                    .AddSingleton(sp => new SchemaRegistryConfig()
                    {
                        Url = schemaRegistryUrl,
                        BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
                        BasicAuthUserInfo = schemaRegistryBasicAuthUserInfo,
                    });
            }
            else
            {
                services
                    .AddSingleton(sp => new ProducerConfig
                    {
                        BootstrapServers = bootstrapServers,
                        ClientId = Dns.GetHostName(),
                        CompressionType = CompressionType.Zstd,
                    })
                    .AddSingleton(sp => new ConsumerConfig
                    {
                        BootstrapServers = bootstrapServers,
                        SslCaLocation = sslCaLocation,
                        GroupId = consumerGroupId
                    })
                    .AddSingleton(sp => new SchemaRegistryConfig()
                    {
                        Url = schemaRegistryUrl,
                    });
            }

            return services
                    .AddSingleton(typeof(IKafkaPublisher<,>), typeof(KafkaPublisher<,>))
                    .AddSingleton<IKafkaConsumer, KafkaConsumer>();
        }
    }
}
