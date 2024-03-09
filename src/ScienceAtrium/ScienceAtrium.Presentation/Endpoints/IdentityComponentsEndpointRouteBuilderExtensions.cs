using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Commands;
using ScienceAtrium.Application.UserAggregate.CustomerAggregate.Queries;
using ScienceAtrium.Domain.RootAggregate.Options;
using ScienceAtrium.Domain.UserAggregate.CustomerAggregate;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Presentation.Components.Account;
using ScienceAtrium.Presentation.Components.Account.Pages.Manage;
using System.Security.Claims;
using System.Text.Json;

namespace Microsoft.AspNetCore.Routing;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    private const string LoginCallbackAction = "LoginCallback";

    private static Guid CustomerId { get; set; } = Guid.Empty;

    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/PerformExternalLogin", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] IDataProtectionProvider idp,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            var properties = await GetAuthenticationPropertiesAsync(context, signInManager, provider, returnUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

		accountGroup.MapPost("/PerformExternalLoginFromQuery", async (
			HttpContext context,
			[FromServices] SignInManager<ApplicationUser> signInManager,
			[FromQuery] string provider,
			[FromQuery] string returnUrl) =>
        {
            var properties = await GetAuthenticationPropertiesAsync(context, signInManager, provider, returnUrl);
			return TypedResults.Challenge(properties, [provider]);
		});

		accountGroup.MapGet("/ExternalLogin", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
			[FromServices] IDataProtectionProvider idp,
            [FromServices] IMediator mediator,
            [FromServices] Serilog.ILogger logger,
            [FromServices] IMapper mapper,
            [FromQuery] string? action,
            [FromQuery] string? remoteError,
            [FromQuery] string? returnUrl) =>
		{
			if (remoteError is not null)
			{
				context.Response.Cookies.Append(IdentityRedirectManager.StatusCookieName, $"Error from external provider: {remoteError}",
					IdentityRedirectManager.GetStatusCookie(context));
				context.Response.Redirect("/Login");
                return;
			}

			var info = await signInManager.GetExternalLoginInfoAsync();
            if (info is null)
			{
				context.Response.Cookies.Append(IdentityRedirectManager.StatusCookieName, "Error loading external login information.",
					IdentityRedirectManager.GetStatusCookie(context));
				context.Response.Redirect("/Login");
				return;
			}

			if (!HttpMethods.IsGet(context.Request.Method))
				return;

			if (action != LoginCallbackAction)
			{
				// We should only reach this page via the login callback, so redirect back to
				// the login page if we get here some other way.
				context.Response.Redirect("/Login");
				return;
			}

			await OnLoginCallbackAsync(mediator, logger, mapper, info);

			var protector = idp.CreateProtector("customer_id");
            
			context.Response.Cookies.Append("customer_id", protector.Protect(CustomerId.ToString()));

            IEnumerable<KeyValuePair<string, StringValues>> query = [
                new("customer_id", protector.Protect(CustomerId.ToString()))
                ];

            var redirectUrl = UriHelper.BuildRelative(context.Request.PathBase, "/home", QueryString.Create(query));
            context.Response.Redirect(redirectUrl);
        });

        accountGroup.MapPost("/Logout", async (
            ClaimsPrincipal user,
            SignInManager<ApplicationUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect($"~/{returnUrl}");
        });

        var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        manageGroup.MapPost("/LinkExternalLogin", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string provider) =>
        {
            // Clear the existing external cookie to ensure a clean login process
            await context.SignOutAsync(IdentityConstants.ExternalScheme);

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/Account/Manage/ExternalLogins",
                QueryString.Create("Action", ExternalLogins.LinkLoginCallbackAction));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
            return TypedResults.Challenge(properties, [provider]);
        });

        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

        manageGroup.MapPost("/DownloadPersonalData", async (
            HttpContext context,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null)
            {
                return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
            }

            var userId = await userManager.GetUserIdAsync(user);
            downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
            var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
        });

        return accountGroup;
    }

    private static async Task<AuthenticationProperties> GetAuthenticationPropertiesAsync(
        HttpContext context,
		SignInManager<ApplicationUser> signInManager,
        string provider,
		string returnUrl)
    {
		IEnumerable<KeyValuePair<string, StringValues>> query = [
				new("ReturnUrl", returnUrl),
				new("Action", LoginCallbackAction)];

		var redirectUrl = UriHelper.BuildRelative(
			context.Request.PathBase,
			"/Account/ExternalLogin",
			QueryString.Create(query));
        
        return signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
	}

    private static async Task OnLoginCallbackAsync(
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
            return;
        }

		var customer = await mediator.Send(
            new GetCustomerQuery(
                new EntityFindOptions<Customer>(predicate: x => x.Email == email)));
        if (!customer.IsEmpty())
        {
            CustomerId = customer.Id;
            logger.Warning("Customer with Id {0} has logged in.", CustomerId);
            return;
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
            return;
        }

        await mediator.Send(new UpdateCachedCustomerCommand(customer));

        CustomerId = customer.Id;

        logger.Information("Created new customer with Id {0}", customer.Id);
    }
}