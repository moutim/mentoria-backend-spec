using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Infrastructure.DependencyInjection;

public static class MediatorInjection
{
    public static IServiceCollection AddMediatorHandlers(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.Load("Application"));

        return services;
    }
}