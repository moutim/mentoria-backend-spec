using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DependencyInjection;

public static class ParameterStoreInjection
{
    public static async Task ConfigureParameterStoreAsync(this IConfigurationManager configuration)
    {
        await configuration.LoadFromParameterStoreAsync(
            "/movimentos/database/connectionstring",
            "ConnectionStrings:DefaultConnection"
        );
    }
}