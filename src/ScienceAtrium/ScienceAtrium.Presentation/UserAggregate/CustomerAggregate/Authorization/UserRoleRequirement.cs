using Microsoft.AspNetCore.Authorization;

namespace ScienceAtrium.Presentation.UserAggregate.CustomerAggregate.Authorization;

public class UserRoleRequirement(string role) : IAuthorizationRequirement
{
    public string Role { get; } = role;
}
