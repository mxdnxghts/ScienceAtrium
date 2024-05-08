using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using ScienceAtrium.Presentation.Components.Account;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using ScienceAtrium.Presentation.UserAggregate.Constants;

namespace ScienceAtrium.Presentation.Components.Authorization;

public abstract class BaseAuthorizeView : AuthorizeView
{
	protected AuthenticationState currentAuthenticationState;
	protected bool isAuthorized;

	[Inject]
	internal AuthenticationStateProvider AuthenticationStateProvider { get; set; }

	[CascadingParameter]
	protected Task<AuthenticationState> AuthenticationStateTask { get; set; }

	protected string? RoleValue { get; private set; }

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		if (currentAuthenticationState == null)
		{
			builder.AddContent(0, Authorizing);
		}
		else if (isAuthorized)
		{
			var authorizedContent = Authorized ?? ChildContent;
			builder.AddContent(1, authorizedContent?.Invoke(currentAuthenticationState));
		}
		else
		{
			builder.AddContent(2, NotAuthorized?.Invoke(currentAuthenticationState));
		}
	}

	protected async Task AuthorizeAsync(string role)
	{
		var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
		currentAuthenticationState = await AuthenticationStateTask;
		var googleIdentity = user.Identities.FirstOrDefault(x => x.AuthenticationType == GoogleDefaults.AuthenticationScheme);
		if (googleIdentity is null)
			return;

		var roleValue = googleIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
		if (roleValue is null)
			return;

		isAuthorized = roleValue.Equals(role, StringComparison.OrdinalIgnoreCase);
	}
}
