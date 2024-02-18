using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Presentation.Components.Account;
using System.Text.Encodings.Web;

namespace ScienceAtrium.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddCascadingAuthenticationState();
        serviceCollection.AddScoped<IdentityUserAccessor>();
        serviceCollection.AddScoped<IdentityRedirectManager>();
        serviceCollection.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        serviceCollection.AddDefaultIdentity<ApplicationUser>(o => o.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        serviceCollection.AddAuthentication()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                //googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
                googleOptions.SaveTokens = true;
                googleOptions.Events = new OAuthEvents
                {
                    OnRemoteFailure = HandleOnRemoteFailure,
                    OnAccessDenied = HandleOnAccessDeniedFailure,
                };
            });
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
}
