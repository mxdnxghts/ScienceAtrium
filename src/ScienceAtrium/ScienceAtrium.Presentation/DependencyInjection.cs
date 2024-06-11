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
using ScienceAtrium.Presentation.Helpers;
using ScienceAtrium.Presentation.UserAggregate;
using ScienceAtrium.Presentation.UserAggregate.Authorization;
using ScienceAtrium.Presentation.UserAggregate.Constants;

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

        GoogleAuthenticationHelper.Configuration = configuration;
        
        serviceCollection
            .AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddGoogle(googleOptions =>
            {
                //googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                //googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                googleOptions.ClientId = Environment.GetEnvironmentVariable("ClientId");
                googleOptions.ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
                googleOptions.SaveTokens = true;

                googleOptions.Events = new OAuthEvents
                {
                    OnRemoteFailure = GoogleAuthenticationHelper.HandleOnRemoteFailure,
                    OnAccessDenied = GoogleAuthenticationHelper.HandleOnAccessDeniedFailure,
                    OnCreatingTicket = GoogleAuthenticationHelper.HandleOnCreatingTicket,
                    OnTicketReceived = GoogleAuthenticationHelper.HandleOnTicketReceived,
                };
			});
        return serviceCollection;
    }

    private static IServiceCollection AddAppAuthorization(this IServiceCollection serviceCollection)
    {
        var policies = GetAuthorizationPolicies();

        serviceCollection.AddAuthorizationBuilder()
            .AddPolicy("default_policy", policies["default_policy"])
            .AddPolicy("executor_policy", policies["executor_policy"])
            .AddPolicy("admin_policy", policies["admin_policy"]);

        serviceCollection.AddScoped<IAuthorizationHandler, UserRoleAuthorizationHandler>();
        return serviceCollection;
    }

    private static Dictionary<string, AuthorizationPolicy> GetAuthorizationPolicies()
    {
        var pb = new AuthorizationPolicyBuilder();
		var defaultPolicy = pb
            .AddAuthenticationSchemes(GoogleDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();

		var googlePolicy = pb
            .Combine(defaultPolicy)
            .AddRequirements(new UserRoleRequirement(UserAuthorizationConstants.AllowedRoles))
            .Build();
        var executorPanelPolicy = pb
            .Combine(defaultPolicy)
            .AddRequirements(new UserRoleRequirement(
                [UserAuthorizationConstants.ExecutorRole, UserAuthorizationConstants.AdminRole]))
            .Build();
        var adminPolicy = pb
            .Combine(defaultPolicy)
            .AddRequirements(new UserRoleRequirement([UserAuthorizationConstants.AdminRole]))
            .Build();

        return new Dictionary<string, AuthorizationPolicy>
        {
            { "default_policy", googlePolicy },
            { "executor_policy", executorPanelPolicy },
            { "admin_policy", adminPolicy }
        };
	}
}
