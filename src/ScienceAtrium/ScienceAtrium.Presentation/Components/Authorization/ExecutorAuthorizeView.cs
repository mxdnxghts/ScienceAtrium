using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using ScienceAtrium.Presentation.UserAggregate.Constants;

namespace ScienceAtrium.Presentation.Components.Authorization;

public class ExecutorAuthorizeView : BaseAuthorizeView
{
	protected override async Task OnParametersSetAsync()
	{
		await AuthorizeAsync(UserAuthorizationConstants.ExecutorRole);
	}
}