using System.Diagnostics.CodeAnalysis;

namespace BitzArt.Blazor.Cookies;

public class Cookie
{
    public required string Name { get; set; }
    public required string Value { get; set; }
    public DateTimeOffset? Expiration { get; set; }

    public bool? Secure { get; set; }
    public bool? HttpOnly { get; set; }
    public string? Domain { get; set; }
    public string? Path { get; set; }

    [SetsRequiredMembers]
    public Cookie(string key, string value, DateTimeOffset? expiration = null)
    {
        Name = key;
        Value = value;
        Expiration = expiration;
    }

    public Cookie()
    {
    }
}
