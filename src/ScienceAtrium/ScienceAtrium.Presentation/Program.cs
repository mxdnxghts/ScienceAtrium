using Microsoft.AspNetCore.Identity;
using ScienceAtrium.Application;
using ScienceAtrium.Infrastructure;
using ScienceAtrium.Infrastructure.Data;
using ScienceAtrium.Presentation;
using ScienceAtrium.Presentation.Components;
using ScienceAtrium.Presentation.Components.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddPresentationAuthentication(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
	app.MapHealthChecks("/health");
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.Run();
