using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using ScienceAtrium.Infrastructure.Extensions;
using ScienceAtrium.Presentation.UserAggregate;
using ScienceAtrium.Presentation.UserAggregate.Authorization;

namespace ScienceAtrium.Presentation.Endpoints;

internal static class CustomRedirectionEndpointRouteBuilderExtension
{
	public static IEndpointConventionBuilder MapRewroteEndpoints(this IEndpointRouteBuilder endpoints)
	{
		var home = endpoints.MapGroup("/");
		
		home.MapGet("/home-redirect", async (
            HttpContext context,
            [FromServices] IDataProtectionProvider idp,
			[FromServices] IDistributedCache cache)
            => await DefaultRedirect(context, idp, cache, "/home"));

		home.MapGet("/account-redirect", async (
			HttpContext context,
			[FromServices] IDataProtectionProvider idp,
			[FromServices] IDistributedCache cache)
			=> await DefaultRedirect(context, idp, cache, "/account"));

		home.MapGet("/basket-redirect", async (
			HttpContext context,
			[FromServices] IDataProtectionProvider idp,
			[FromServices] IDistributedCache cache)
			=> await DefaultRedirect(context, idp, cache, "/basket"));

		return home;
	}

	public static async Task DefaultRedirect(HttpContext context,
									IDataProtectionProvider idp,
									IDistributedCache cache,
									PathString path)
    {
		var cookieUserId = context.Request.Cookies["user_id"];
		var cookieEmail = context.Request.Cookies["user_email"];
		if (cookieUserId is null && cookieEmail is not null)
		{
			var protector = idp.CreateProtector(UserConstants.DataProtectorPurpose);
			var claimUserId = await cache.GetRecordAsync<Guid>($"{nameof(UserRoleRequirement)}_{protector.Unprotect(cookieEmail)}");
			if (!claimUserId.Equals(Guid.Empty))
			{
				var protectedClaimUserId = protector.Protect(claimUserId.ToString());
				context.Response.Cookies.Append("user_id", protectedClaimUserId);
				cookieUserId = protectedClaimUserId;
			}
		}

		IEnumerable<KeyValuePair<string, StringValues>> query = [
			new ("user_email", cookieEmail),
			new ("user_id", cookieUserId)
		];

		var redirectUrl = UriHelper.BuildRelative(GetOriginalPath(context.Request.PathBase), path, QueryString.Create(query));
		context.Response.Redirect(redirectUrl);
	}

	private static string GetOriginalPath(PathString redirectPathString)
	{
		return new string(redirectPathString.ToString().ToCharArray().TakeWhile(x => x != '-').ToArray());
	}
}
