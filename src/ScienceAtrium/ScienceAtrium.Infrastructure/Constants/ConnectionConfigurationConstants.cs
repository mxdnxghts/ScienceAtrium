namespace ScienceAtrium.Infrastructure.Constants;
public static class ConnectionConfigurationConstants
{
    public const string DevelopmentConnectionString = "Database";
    public const string ProductionConnectionString = "DatabaseRelease";
    public const string DevelopmentConnectionStringRedis = "RedisCache";
    public const string ProductionConnectionStringRedis = "RedisCacheRelease";
}
