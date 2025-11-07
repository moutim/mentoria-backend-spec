using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public interface IParameterStoreService
{
    Task<string> GetParameterAsync(string parameterName);
    Task<Dictionary<string, string>> GetParametersByPathAsync(string path);
}

public class ParameterStoreService : IParameterStoreService
{
    private readonly IAmazonSSM _ssmClient;
    private readonly IMemoryCache _cache;
    private const int CacheDurationMinutes = 60;

    public ParameterStoreService(IAmazonSSM ssmClient, IMemoryCache cache)
    {
        _ssmClient = ssmClient;
        _cache = cache;
    }

    public async Task<string> GetParameterAsync(string parameterName)
    {
        var cacheKey = $"param_{parameterName}";
        
        if (_cache.TryGetValue(cacheKey, out string? cachedValue))
        {
            return cachedValue ?? string.Empty;
        }

        try
        {
            var request = new GetParameterRequest
            {
                Name = parameterName,
                WithDecryption = true
            };

            var response = await _ssmClient.GetParameterAsync(request);
            var value = response.Parameter.Value;

            _cache.Set(cacheKey, value, TimeSpan.FromMinutes(CacheDurationMinutes));

            return value;
        }
        catch (ParameterNotFound)
        {
            throw new InvalidOperationException($"Parameter '{parameterName}' not found");
        }
    }

    public async Task<Dictionary<string, string>> GetParametersByPathAsync(string path)
    {
        var cacheKey = $"params_path_{path}";
        
        if (_cache.TryGetValue(cacheKey, out Dictionary<string, string>? cachedParams))
        {
            return cachedParams ?? new Dictionary<string, string>();
        }

        var parameters = new Dictionary<string, string>();

        try
        {
            var request = new GetParametersByPathRequest
            {
                Path = path,
                Recursive = true,
                WithDecryption = true
            };

            GetParametersByPathResponse response;
            do
            {
                response = await _ssmClient.GetParametersByPathAsync(request);

                foreach (var parameter in response.Parameters)
                {
                    var key = parameter.Name.Substring(parameter.Name.LastIndexOf('/') + 1);
                    parameters[key] = parameter.Value;
                }

                request.NextToken = response.NextToken;
            } while (!string.IsNullOrEmpty(response.NextToken));

            _cache.Set(cacheKey, parameters, TimeSpan.FromMinutes(CacheDurationMinutes));

            return parameters;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving parameters from path '{path}'", ex);
        }
    }
}
using Amazon.SimpleSystemsManagement;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class ParameterStoreInjection
{
    public static void AddParameterStore(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonSSM>(new AmazonSimpleSystemsManagementClient());
        services.AddSingleton<IParameterStoreService, ParameterStoreService>();
    }
}

