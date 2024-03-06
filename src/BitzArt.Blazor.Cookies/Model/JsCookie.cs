namespace BitzArt.Blazor.Cookies;

internal class JsCookie
{
    public string? Name { get; set; }
    public string? Value { get; set; }
    public double? Expires { get; set; }
    public bool? Secure { get; set; }
    public bool? HttpOnly { get; set; }
    public string? Domain { get; set; }
    public string? Path { get; set; }

    public JsCookie(Cookie cookie)
    {
        Name = cookie.Name;
        Value = cookie.Value;
        Expires = cookie.Expiration?.ToUnixTimeMilliseconds();
        Secure = cookie.Secure;
        HttpOnly = cookie.HttpOnly;
        Domain = cookie.Domain;
        Path = cookie.Path ?? "/";
    }

    public JsCookie()
    {
    }

    public Cookie Parse()
    {
        DateTimeOffset? expiration = Expires.HasValue 
            ? new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(Expires!.Value) 
            : null;

        return new Cookie()
        {
            Name = Name ?? string.Empty,
            Value = Value ?? string.Empty,
            Expiration = expiration,
            Secure = Secure,
            HttpOnly = HttpOnly,
            Domain = Domain,
            Path = Path
        };
    }
}