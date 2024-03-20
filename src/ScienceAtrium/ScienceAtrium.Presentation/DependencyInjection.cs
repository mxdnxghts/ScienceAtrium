using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Presentation.Components.Account;
using ScienceAtrium.Presentation.UserAggregate;
using ScienceAtrium.Presentation.UserAggregate.Authorization;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ScienceAtrium.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDefaultIdentity<ApplicationUser>(o => o.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        serviceCollection.AddAppAuthentication(configuration);
        serviceCollection.AddAppAuthorization();

        serviceCollection.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        serviceCollection.AddHttpClient("AccountLoginClient", o =>
        {
            o.BaseAddress = new Uri(configuration.GetRequiredSection("Clients:AccountLoginClient")?.Value
                ?? throw new ArgumentNullException("AcountLoginClient"));
        });

        return serviceCollection;
    }

    private static IServiceCollection AddAppAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddCascadingAuthenticationState();
        serviceCollection.AddDataProtection()
            .SetApplicationName(UserConstants.DataProtectionApplicationName)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(60));
            
        serviceCollection.AddScoped<IdentityUserAccessor>();
        serviceCollection.AddScoped<IdentityRedirectManager>();
        serviceCollection.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        serviceCollection
            .AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                googleOptions.SaveTokens = true;

                googleOptions.Events = new OAuthEvents
                {
                    OnRemoteFailure = HandleOnRemoteFailure,
                    OnAccessDenied = HandleOnAccessDeniedFailure,
                    OnCreatingTicket = AddRoleClaim,
                    OnTicketReceived = HandleOnTicketReceived,
                };
			});
        return serviceCollection;
    }

    private static IServiceCollection AddAppAuthorization(this IServiceCollection serviceCollection)
    {
        IAuthorizationRequirement[] requirements = 
            [
                new UserRoleRequirement(UserAuthorizationConstants.CustomerRole),
                new DenyAnonymousAuthorizationRequirement(),
            ];
        serviceCollection.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder(GoogleDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build())
            .AddPolicy("google-oauth", pb =>
            {
                pb.AddAuthenticationSchemes(GoogleDefaults.AuthenticationScheme)
                    .AddRequirements(requirements);
            });
        serviceCollection.AddSingleton<IAuthorizationHandler, UserRoleAuthorizationHandler>();
        return serviceCollection;
    }
    private static async Task HandleOnRemoteFailure(RemoteFailureContext context)
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

    private static async Task HandleOnAccessDeniedFailure(AccessDeniedContext context)
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

    private static async Task HandleOnTicketReceived(TicketReceivedContext context)
    {
        var idp = DataProtectionProvider.Create("ScienceAtrium");
        var protector = idp.CreateProtector(UserConstants.DataProtectorPurpose);

        var userEmailClaim = context.Principal.Claims.First(claim => claim.Type == ClaimTypes.Email);
        var cookieOptions = new CookieOptions()
        {
            SameSite = SameSiteMode.Strict,
            IsEssential = true,
            MaxAge = TimeSpan.FromDays(360),
        };

        context.HttpContext.Response.Cookies.Append("user_email", protector.Protect(userEmailClaim.Value), cookieOptions);

        IEnumerable<KeyValuePair<string, StringValues>> query = [
            new("user_email", protector.Protect(userEmailClaim.Value)),
            new ("user_id", protector.Protect(Guid.Empty.ToString()))
        ];
        var returnUri = UriHelper.BuildRelative(context.Request.PathBase, "/home", QueryString.Create(query));
        context.ReturnUri = returnUri;
    }

    private static Task AddRoleClaim(OAuthCreatingTicketContext context)
    {
        var googleIdentity = context.Principal.Identities.First(identity => identity.AuthenticationType == GoogleDefaults.AuthenticationScheme);

        var roleClaim = new Claim(ClaimTypes.Role, UserAuthorizationConstants.CustomerRole);
        var userEmailClaim = new Claim(ClaimTypes.Email, googleIdentity.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value);
        
        if (!googleIdentity.Claims.Any(claim => claim.Type == ClaimTypes.Role))
            googleIdentity.AddClaim(roleClaim);

        if (!googleIdentity.Claims.Any(claim => claim.Type == ClaimTypes.UserData))
            googleIdentity.AddClaim(userEmailClaim);

        return Task.CompletedTask;
    }
}
