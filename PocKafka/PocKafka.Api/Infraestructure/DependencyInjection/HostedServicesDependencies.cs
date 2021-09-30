using Microsoft.Extensions.DependencyInjection;
using PocKafka.Api.HostedServices;

namespace PocKafka.Api.Infraestructure.DependencyInjection
{
    public static class HostedServicesDependencies
    {
        public static IServiceCollection AddHostedServicesDependencies(this IServiceCollection services) => 
            services.AddHostedService<ConsumerHostedService>();
    }
}
