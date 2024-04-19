using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace ScienceAtrium.Presentation.Helpers;

public static class NavigationHelper
{
    public static string GetErrorUri(string errorMessage, int statusCode)
    {
        IEnumerable<KeyValuePair<string, StringValues>> query = [
            new ("error_msg", errorMessage),
            new ("error_status", statusCode.ToString())
            ];

        var redirectUri = UriHelper.BuildRelative("", "/error", QueryString.Create(query));
        return redirectUri;
    }
}
