using Microsoft.AspNetCore.Authorization;

namespace ScienceAtrium.Presentation.UserAggregate.CustomerAggregate.Authorization;

public class CustomerRoleRequirement(string role) : IAuthorizationRequirement
{
    public string Role { get; } = role;
}
