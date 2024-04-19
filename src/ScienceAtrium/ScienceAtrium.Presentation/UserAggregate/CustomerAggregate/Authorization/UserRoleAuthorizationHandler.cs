using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using ScienceAtrium.Domain.RootAggregate.Interfaces;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Infrastructure.Extensions;
using System.Security.Claims;

namespace ScienceAtrium.Presentation.UserAggregate.CustomerAggregate.Authorization;

public class UserRoleAuthorizationHandler(IReaderAsync<Customer> _customerReader, IDistributedCache _cache)
    : AuthorizationHandler<UserRoleRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                         UserRoleRequirement requirement)
    {
        if (context.User is null || requirement is null)
        {
            context.Fail(
                new AuthorizationFailureReason(this, $"{nameof(context.User)} or {nameof(UserRoleRequirement)} is null"));
            return;
        }

        var googleIdentity = context.User
            .Identities
            .FirstOrDefault(identity => identity.AuthenticationType == GoogleDefaults.AuthenticationScheme);
        if (googleIdentity is null)
        {
            context.Fail(new AuthorizationFailureReason(this, new ArgumentNullException(nameof(googleIdentity)).Message));
            return;
        }

        var roleClaim = googleIdentity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
        if (roleClaim?.Value != requirement.Role)
        {
            context.Fail(
                new AuthorizationFailureReason(this, $"{nameof(UserRoleRequirement)} inconsistency"));
            return;
        }

        var userEmailClaim = googleIdentity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
        if (userEmailClaim is null)
        {
            context.Fail(
                new AuthorizationFailureReason(this, new ArgumentNullException("userEmailClaim").Message));
            return;
        }

        var userId = (await _customerReader.GetAsync(
        new EntityFindOptions<Customer>(predicate: c => c.Email == userEmailClaim.Value))).Id;
        await _cache.SetRecordAsync($"{nameof(UserRoleRequirement)}_{userEmailClaim.Value}", userId);

        if (!googleIdentity.Claims.Any(claim => claim.Type == ClaimTypes.Sid))
            googleIdentity.AddClaim(new Claim(ClaimTypes.Sid, userId.ToString()));

        context.Succeed(requirement);
    }
}
