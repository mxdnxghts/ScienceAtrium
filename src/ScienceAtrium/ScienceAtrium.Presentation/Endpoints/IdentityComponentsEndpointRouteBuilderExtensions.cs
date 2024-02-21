using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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

        accountGroup.MapPost("/PerformExternalLogin", (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            IEnumerable<KeyValuePair<string, StringValues>> query = [
                new("ReturnUrl", returnUrl),
                new("Action", LoginCallbackAction)];

            var redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/Account/ExternalLogin",
                QueryString.Create(query));

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

        accountGroup.MapGet("/ExternalLogin", async (
            HttpContext context,
            [FromServices] SignInManager<ApplicationUser> signInManager,
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
                context.Response.Redirect("Account/Login");
                return;
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                context.Response.Cookies.Append(IdentityRedirectManager.StatusCookieName, "Error loading external login information.",
                    IdentityRedirectManager.GetStatusCookie(context));
                context.Response.Redirect("Account/Login");
                return;
            }

            if (HttpMethods.IsGet(context.Request.Method))
            {
                if (action == LoginCallbackAction)
                {
                    await OnLoginCallbackAsync(mediator, logger, mapper, info);
                    IEnumerable<KeyValuePair<string, StringValues>> query = [
                        new(nameof(CustomerId), CustomerId.ToString())];

                    context.Response.Cookies.Append($"{nameof(CustomerId)}_{CustomerId}", CustomerId.ToString(),
                        new CookieOptions()
                        {
                            SameSite = SameSiteMode.Strict,
                            IsEssential = true,
                            MaxAge = TimeSpan.FromDays(7)
                        });

                    var redirectUrl = UriHelper.BuildRelative(context.Request.PathBase, "/", QueryString.Create(query));
                    context.Response.Redirect(redirectUrl);
                    return;
                }

                // We should only reach this page via the login callback, so redirect back to
                // the login page if we get here some other way.
                context.Response.Redirect("Account/Login");
            }
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

        var customer = await mediator.Send(
            new GetCustomerQuery(
                new EntityFindOptions<Customer>(predicate: x => x.Email == email)));
        if (!customer.IsEmpty())
        {
            CustomerId = customer.Id;
            logger.Information("Customer with Id {0} has logged in.", CustomerId);
            return;
        }

        var user = new Customer(Guid.NewGuid())
            .UpdateName(externalLoginInfo
                .Principal
                .Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? throw new ArgumentNullException("Name"))
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