using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ScienceAtrium.Presentation.UserAggregate.Authorization;

public class UserRoleAuthorizationHandler : AuthorizationHandler<UserRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                         UserRoleRequirement requirement)
    {
        if (context.User is null || requirement is null)
        {
            context.Fail(
                new AuthorizationFailureReason(this, $"{nameof(context.User)} or {nameof(UserRoleRequirement)} is null"));
            return Task.CompletedTask;
        }

        var googleIdentity = context.User
            .Identities
            .First(identity => identity.AuthenticationType == GoogleDefaults.AuthenticationScheme);
        var roleClaim = googleIdentity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
        if (roleClaim?.Value != requirement.Role)
        {
            context.Fail(
                new AuthorizationFailureReason(this, $"{nameof(UserRoleRequirement)} inconsistency"));
            return Task.CompletedTask;
        }

        var userEmailClaim = googleIdentity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
        if (userEmailClaim is null)
        {
            context.Fail(
                new AuthorizationFailureReason(this, new ArgumentNullException("userDataClaim").Message));
            return Task.CompletedTask;
        }
        
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
