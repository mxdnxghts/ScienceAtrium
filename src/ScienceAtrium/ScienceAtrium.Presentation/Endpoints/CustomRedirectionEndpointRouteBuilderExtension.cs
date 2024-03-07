using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace ScienceAtrium.Presentation.Endpoints;

internal static class CustomRedirectionEndpointRouteBuilderExtension
{
	public static IEndpointConventionBuilder MapRewroteEndpoints(this IEndpointRouteBuilder endpoints)
	{
		var home = endpoints.MapGroup("/");
		
		home.MapGet("/home-redirect", async (context) => DefaultRedirect(context, "/home"));
		home.MapGet("/account-redirect", async (context) => DefaultRedirect(context, "/account"));
		home.MapGet("/basket-redirect", async (context) => DefaultRedirect(context, "/basket"));

		return home;
	}

	private static void DefaultRedirect(HttpContext context, PathString path)
    {
        IEnumerable<KeyValuePair<string, StringValues>> query = [
					new ("CustomerId", context.Request.Cookies["CustomerId"] ?? Guid.Empty.ToString())
				];
		var redirectUrl = UriHelper.BuildRelative(GetOriginalPath(context.Request.PathBase), path, QueryString.Create(query));
		context.Response.Redirect(redirectUrl);
		
	}


    private static string GetOriginalPath(PathString redirectPathString)
	{
		return new string(redirectPathString.ToString().ToCharArray().TakeWhile(x => x != '-').ToArray());
	}
}
