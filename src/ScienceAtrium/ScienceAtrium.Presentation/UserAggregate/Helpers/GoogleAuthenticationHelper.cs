using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using ScienceAtrium.Presentation.UserAggregate.Constants;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ScienceAtrium.Presentation.UserAggregate.Helpers;

public static class GoogleAuthenticationHelper
{
	public static Task HandleOnTicketReceived(TicketReceivedContext context)
	{
		var idp = DataProtectionProvider.Create(UserConstants.DataProtectionApplicationName);
		var protector = idp.CreateProtector(UserConstants.DataProtectorPurpose);

		var userIdClaim = context.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
		var userEmailClaim = context.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
		var userNameClaim = context.Principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

		context.HttpContext.Response.Cookies.Append("user_id", protector.Protect(userIdClaim?.Value ?? ""), CookieConstants.CookieOptions);
		context.HttpContext.Response.Cookies.Append("user_email", protector.Protect(userEmailClaim?.Value ?? ""), CookieConstants.CookieOptions);

		IList<KeyValuePair<string, StringValues>> query = [
			new("user_email", protector.Protect(userEmailClaim.Value ?? "")),
			new ("user_id", protector.Protect(userIdClaim?.Value ?? "")),
		];

		// context.ReturnUri has a view "/smth?.." and that's why we take string before param split symbol "?"
		var destPath = new string(context.ReturnUri?.TakeWhile(c => c != '?').ToArray());
		var returnUri = UriHelper.BuildRelative(context.Request.PathBase, destPath, QueryString.Create(query));

		query.Add(new("return_uri", returnUri));
		query.Add(new ("user_name", protector.Protect(userNameClaim?.Value ?? "")));

		context.ReturnUri = UriHelper.BuildRelative(context.Request.PathBase, "/sign-in", QueryString.Create(query));
		context.Success();

		return Task.CompletedTask;
	}

	public static Task AddRoleClaims(OAuthCreatingTicketContext context)
	{
		var googleIdentity = context.Principal.Identities.First(identity => identity.AuthenticationType == GoogleDefaults.AuthenticationScheme);

		var roleClaim = new Claim(ClaimTypes.Role, UserAuthorizationConstants.CustomerRole);

		if (!googleIdentity.Claims.Any(claim => claim.Type == ClaimTypes.Role))
			googleIdentity.AddClaim(roleClaim);

		context.Success();

		return Task.CompletedTask;
	}
	
	public static async Task HandleOnRemoteFailure(RemoteFailureContext context)
	{
		context.Response.StatusCode = 500;
		context.Response.ContentType = "text/html";
		await context.Response.WriteAsync("<html><body>");
		await context.Response.WriteAsync("A remote failure has occurred: <br>" +
			context.Failure.Message.Split(Environment.NewLine).Select(s => HtmlEncoder.Default.Encode(s) + "<br>").Aggregate((s1, s2) => s1 + s2));

		foreach (var header in context.Response.Headers)
			await context.Response.WriteAsync($"{header.Key}:{header.Value}");

		if (context.Properties != null)
		{
			await context.Response.WriteAsync("Properties:<br>");
			foreach (var pair in context.Properties.Items)
			{
				await context.Response.WriteAsync($"-{HtmlEncoder.Default.Encode(pair.Key)}={HtmlEncoder.Default.Encode(pair.Value)}<br>");
			}
		}

		await context.Response.WriteAsync("<a href=\"/\">Home</a>");
		await context.Response.WriteAsync("</body></html>");

		context.Response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.Encode(context.Failure.Message));

		context.HandleResponse();
	}

	public static async Task HandleOnAccessDeniedFailure(AccessDeniedContext context)
	{
		context.Response.StatusCode = 500;
		context.Response.ContentType = "text/html";
		await context.Response.WriteAsync("<html><body>");
		await context.Response.WriteAsync($"{nameof(context.AccessDeniedPath)}: {context.AccessDeniedPath}");

		foreach (var header in context.Response.Headers)
			await context.Response.WriteAsync($"{header.Key}:{header.Value}");

		if (context.Properties != null)
		{
			await context.Response.WriteAsync("Properties:<br>");
			foreach (var pair in context.Properties.Items)
			{
				await context.Response.WriteAsync($"-{HtmlEncoder.Default.Encode(pair.Key)}={HtmlEncoder.Default.Encode(pair.Value)}<br>");
			}
		}

		await context.Response.WriteAsync("<a href=\"/\">Home</a>");
		await context.Response.WriteAsync("</body></html>");

		context.Response.Redirect("/error?AccessDenied=");

		context.HandleResponse();
	}
}
