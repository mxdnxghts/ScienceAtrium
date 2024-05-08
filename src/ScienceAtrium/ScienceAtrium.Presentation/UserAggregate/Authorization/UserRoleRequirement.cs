using Microsoft.AspNetCore.Authorization;

namespace ScienceAtrium.Presentation.UserAggregate.Authorization;

public record UserRoleRequirement(List<string> Roles) : IAuthorizationRequirement;
