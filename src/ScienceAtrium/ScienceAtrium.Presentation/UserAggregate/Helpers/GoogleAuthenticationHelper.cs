using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using ScienceAtrium.Presentation.UserAggregate.Authorization;
using ScienceAtrium.Presentation.UserAggregate.Constants;
using System.Security.Claims;
using System.Text.Encodings.Web;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Commands;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;

namespace ScienceAtrium.Presentation.UserAggregate.Helpers;

public static class GoogleAuthenticationHelper
{
	private static Guid CustomerId { get; set; }

	public static async Task HandleOnTicketReceived(TicketReceivedContext context)
	{
		var idp = DataProtectionProvider.Create(UserConstants.DataProtectionApplicationName);
		var protector = idp.CreateProtector(UserConstants.DataProtectorPurpose);

		var userEmailClaim = context.Principal.Claims.First(claim => claim.Type == ClaimTypes.Email);

		context.HttpContext.Response.Cookies.Append("user_email", protector.Protect(userEmailClaim.Value), CookieConstants.CookieOptions);

		IEnumerable<KeyValuePair<string, StringValues>> query = [
			new("user_email", protector.Protect(userEmailClaim.Value)),
			new ("user_id", protector.Protect(Guid.Empty.ToString()))
		];
		var returnUri = UriHelper.BuildRelative(context.Request.PathBase, "/home", QueryString.Create(query));
		context.ReturnUri = returnUri;
	}

	public static Task AddRoleClaim(OAuthCreatingTicketContext context)
	{
		var googleIdentity = context.Principal.Identities.First(identity => identity.AuthenticationType == GoogleDefaults.AuthenticationScheme);

		var roleClaim = new Claim(ClaimTypes.Role, UserAuthorizationConstants.CustomerRole);
		var userEmailClaim = new Claim(ClaimTypes.Email, googleIdentity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value);

		if (!googleIdentity.Claims.Any(claim => claim.Type == ClaimTypes.Role))
			googleIdentity.AddClaim(roleClaim);

		if (!googleIdentity.Claims.Any(claim => claim.Type == ClaimTypes.Email))
			googleIdentity.AddClaim(userEmailClaim);

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

	private static async Task<bool> AddUserToSystem(
	IMediator mediator,
	Serilog.ILogger logger,
	IMapper mapper,
	ExternalLoginInfo externalLoginInfo)
	{
		var email = externalLoginInfo
				.Principal
				.Claims
				.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? throw new ArgumentNullException("Email");

		var name = externalLoginInfo
				.Principal
				.Claims
				.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? throw new ArgumentNullException("Name");

		if (externalLoginInfo.Principal.Identities.FirstOrDefault(_ => _.Name == name)?.IsAuthenticated == false)
		{
			logger.Warning("Couldn't authenticate with {0} provider the user {1}", externalLoginInfo.ProviderDisplayName, name);
			return false;
		}

		var customer = await mediator.Send(
			new GetCustomerQuery(
				new EntityFindOptions<Customer>(predicate: x => x.Email == email)));
		if (!customer.IsEmpty())
		{
			CustomerId = customer.Id;
			logger.Warning("Customer with Id {0} has logged in.", CustomerId);
			return false;
		}

		var user = new Customer(Guid.NewGuid())
			.UpdateName(name)
			.UpdateEmail(email)
			.UpdatePhoneNumber("null");

		customer = mapper.Map<Customer>(user);

		var customerCreationResult = await mediator.Send(new CreateCustomerCommand(customer));
		if (customerCreationResult <= 0)
		{
			logger.Error("Didn't create new customer with Id {0}. Count of changes {1}", customer.Id, customerCreationResult);
			return false;
		}

		await mediator.Send(new UpdateCachedCustomerCommand(customer));

		CustomerId = customer.Id;

		logger.Information("Created new customer with Id {0}", customer.Id);

		return true;
	}
}
