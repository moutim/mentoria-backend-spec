using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration;

public static class AwsParameterStoreConfiguration
{
    public static async Task LoadFromParameterStoreAsync(
        this IConfigurationManager configuration,
        string parameterName,
        string appSettingsKey)
    {
        using var ssmClient = CreateSsmClient(configuration);
        var parameterValue = await GetParameterAsync(ssmClient, parameterName);
        configuration[appSettingsKey] = parameterValue;
    }

    private static AmazonSimpleSystemsManagementClient CreateSsmClient(IConfiguration configuration)
    {
        var awsRegion = configuration["AWS:Region"] ?? "us-east-1";
        var serviceUrl = configuration["AWS:ServiceURL"];
        var accessKey = configuration["AWS:AccessKey"];
        var secretKey = configuration["AWS:SecretKey"];

        var config = new AmazonSimpleSystemsManagementConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsRegion)
        };

        if (!string.IsNullOrEmpty(serviceUrl))
        {
            config.ServiceURL = serviceUrl;
        }

        if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
        {
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            return new AmazonSimpleSystemsManagementClient(credentials, config);
        }

        return new AmazonSimpleSystemsManagementClient(config);
    }

    private static async Task<string> GetParameterAsync(
        IAmazonSimpleSystemsManagement ssmClient,
        string parameterName)
    {
        var request = new GetParameterRequest
        {
            Name = parameterName,
            WithDecryption = true
        };

        var response = await ssmClient.GetParameterAsync(request);
        return response.Parameter.Value;
    }
}