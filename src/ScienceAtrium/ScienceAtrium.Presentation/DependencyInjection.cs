using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Presentation.Components.Account;
using ScienceAtrium.Presentation.UserAggregate;
using ScienceAtrium.Presentation.UserAggregate.Authorization;
using ScienceAtrium.Presentation.UserAggregate.Helpers;

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
                    OnRemoteFailure = GoogleAuthenticationHelper.HandleOnRemoteFailure,
                    OnAccessDenied = GoogleAuthenticationHelper.HandleOnAccessDeniedFailure,
                    OnCreatingTicket = GoogleAuthenticationHelper.AddRoleClaim,
                    OnTicketReceived = GoogleAuthenticationHelper.HandleOnTicketReceived,
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
            //.SetFallbackPolicy(new AuthorizationPolicyBuilder(GoogleDefaults.AuthenticationScheme)
            //    .RequireAuthenticatedUser()
            //    .Build())
            .AddPolicy("google-oauth", pb =>
            {
                pb.AddAuthenticationSchemes(GoogleDefaults.AuthenticationScheme)
                    .AddRequirements(requirements);
            });
        serviceCollection.AddScoped<IAuthorizationHandler, UserRoleAuthorizationHandler>();
        return serviceCollection;
    }
}
