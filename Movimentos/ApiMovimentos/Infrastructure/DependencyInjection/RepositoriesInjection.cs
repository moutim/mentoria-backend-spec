using Application.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class RepositoriesInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMovimentoRepository, MovimentoRepository>();

        return services;
    }
}