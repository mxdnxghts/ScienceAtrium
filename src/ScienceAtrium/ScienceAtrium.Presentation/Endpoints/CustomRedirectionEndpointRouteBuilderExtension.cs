using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ScienceAtrium.Presentation.Endpoints;

internal static class CustomRedirectionEndpointRouteBuilderExtension
{
	public static IEndpointConventionBuilder MapRewroteEndpoints(this IEndpointRouteBuilder endpoints)
	{
		var home = endpoints.MapGroup("/");
		
		home.MapGet("/home-redirect", async (
			HttpContext context,
			[FromServices] IDataProtectionProvider idp)
			=> DefaultRedirect(context, idp, "/home"));

		home.MapGet("/account-redirect", async (
			HttpContext context,
			[FromServices] IDataProtectionProvider idp)
			=> DefaultRedirect(context, idp, "/account"));

		home.MapGet("/basket-redirect", async (
			HttpContext context,
			[FromServices] IDataProtectionProvider idp)
			=> DefaultRedirect(context, idp, "/basket"));

		return home;
	}

	public static void DefaultRedirect(HttpContext context, IDataProtectionProvider idp, PathString path)
    {
		var protector = idp.CreateProtector("customer_id");

		IEnumerable<KeyValuePair<string, StringValues>> query = [
					new ("customer_id", context.Request.Cookies["customer_id"]
					?? protector.Protect(Guid.Empty.ToString()))
				];
		var redirectUrl = UriHelper.BuildRelative(GetOriginalPath(context.Request.PathBase), path, QueryString.Create(query));
		context.Response.Redirect(redirectUrl);
		
	}

    private static string GetOriginalPath(PathString redirectPathString)
	{
		return new string(redirectPathString.ToString().ToCharArray().TakeWhile(x => x != '-').ToArray());
	}
}
