using System.Text;
using System.Web;

namespace BitzArt.Blazor.Cookies;

internal static class CookieStringBuilder
{
    public static string Build(Cookie cookie)
    {
        var builder = new StringBuilder();

        var keyEncoded = HttpUtility.UrlEncode(cookie.Key);
        var valueEncoded = HttpUtility.UrlEncode(cookie.Value);

        builder.Append($"{keyEncoded}={valueEncoded}; ");
        builder.Append($"expires={cookie.Expiration:R}; ");
        builder.Append("path=/");

        if (cookie.SameSiteMode.HasValue)
        {
            builder.Append("; ");
            builder.Append($"SameSite={cookie.SameSiteMode.Value.ToString()}");
        }

        return builder.ToString();
    }
}
