using Infrastructure.Services;

namespace Infrastructure.Configuration;

public static class ParameterStoreHelper
{
    public static class ParameterNames
    {
        public const string DatabaseConnection = "/movimentos/database/connection";
        public const string DatabaseHost = "/movimentos/database/host";
        public const string DatabasePort = "/movimentos/database/port";
        public const string DatabaseUsername = "/movimentos/database/username";
        public const string DatabasePassword = "/movimentos/database/password";
        public const string JwtSecret = "/movimentos/security/jwt-secret";
        public const string ApiKey = "/movimentos/security/api-key";
        public const string EnvironmentName = "/movimentos/app/environment";
        public const string LogLevel = "/movimentos/app/log-level";
    }

    public static async Task<string> GetParameter(IParameterStoreService parameterStore, string parameterName)
    {
        return await parameterStore.GetParameterAsync(parameterName);
    }

    public static async Task<Dictionary<string, string>> GetParametersByPath(IParameterStoreService parameterStore, string path)
    {
        return await parameterStore.GetParametersByPathAsync(path);
    }
}

