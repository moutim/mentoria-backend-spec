using Amazon.SQS;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon;

namespace Infrastructure.DependencyInjection;

public static class EventBusInjection
{
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAmazonSQS>(provider =>
        {
            var awsOptions = configuration.GetSection("AWS");
            var serviceUrl = awsOptions["ServiceURL"];
            var region = awsOptions["Region"] ?? "us-east-1";

            var config = new AmazonSQSConfig
            {
                ServiceURL = serviceUrl,
                UseHttp = !string.IsNullOrEmpty(serviceUrl),
                AuthenticationRegion = region
            };

            if (!string.IsNullOrEmpty(serviceUrl))
            {
                return new AmazonSQSClient("dummy", "dummy", config);
            }

            return new AmazonSQSClient(config);
        });

        services.AddScoped<IEventBus, SqsEventBus>();

        return services;
    }
}
