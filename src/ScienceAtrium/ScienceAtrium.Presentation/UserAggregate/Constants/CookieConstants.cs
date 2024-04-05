namespace ScienceAtrium.Presentation.UserAggregate.Constants;

public static class CookieConstants
{
	public static readonly CookieOptions CookieOptions = new()
	{
		SameSite = SameSiteMode.Strict,
		IsEssential = true,
		MaxAge = TimeSpan.FromDays(360),
	};
}
