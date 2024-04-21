namespace ScienceAtrium.Infrastructure.Constants;
public static class ConnectionConfigurationConstants
{
    public const string DevelopmentConnectionString = "MSSQL";
    public const string ProductionConnectionString = "ScienceAtriumRelease";
    public const string DevelopmentConnectionStringRedis = "ScienceAtriumRedisCache";
    public const string ProductionConnectionStringRedis = "ScienceAtriumRedisCacheRelease";
}
