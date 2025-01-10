using System.Text;

namespace BitzArt.Blazor.Cookies;

internal static class JsCommand
{
    public static string SetCookie(Cookie cookie)
    {
        var builder = new StringBuilder();

        builder.Append("document.cookie = \"");

        builder.Append($"{cookie.Key}={cookie.Value}; ");
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
