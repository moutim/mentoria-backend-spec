namespace Infrastructure.Configuration;

public class ParameterStoreConfiguration
{
    public class DatabaseSettings
    {
        public string? Connection { get; set; }
        public string? Host { get; set; }
        public string? Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class SecuritySettings
    {
        public string? JwtSecret { get; set; }
        public string? ApiKey { get; set; }
    }

    public class AppSettings
    {
        public string? Environment { get; set; }
        public string? LogLevel { get; set; }
    }
}

