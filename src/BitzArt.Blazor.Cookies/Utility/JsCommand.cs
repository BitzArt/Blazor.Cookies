using System.Text;
using System.Web;

namespace BitzArt.Blazor.Cookies;

internal static class JsCommand
{
    public static string SetCookie(Cookie cookie)
    {
        var builder = new StringBuilder();

        builder.Append("document.cookie = \"");

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

        builder.Append('\"');

        return builder.ToString();
    }
}
