using System.Diagnostics.CodeAnalysis;

namespace BitzArt.Blazor.Cookies;

internal class JsCookie
{
    public string? Name { get; set; }
    public string? Value { get; set; }
    public double? Expires { get; set; }
    public string? Domain { get; set; }
    public string? Path { get; set; }

    public JsCookie(string name, string value, DateTimeOffset? expiration = null)
    {
        Name = name;
        Value = value;
        Expires = expiration?.ToUnixTimeMilliseconds();
        Path = "/";
    }

    public JsCookie()
    {
    }

    public Cookie Parse()
    {
        DateTimeOffset? expiration = Expires.HasValue
            ? new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(Expires!.Value)
            : null;

        return new Cookie(Name!, Value!, expiration);
    }
}