namespace ScienceAtrium.Presentation.UserAggregate.Constants;

public static class UserAuthorizationConstants
{
    public const string CustomerRole = "customer";
    public const string ExecutorRole = "executor";
    public const string AdminRole = "admin";
    public static readonly List<string> AllowedRoles = [CustomerRole, ExecutorRole, AdminRole];
}
