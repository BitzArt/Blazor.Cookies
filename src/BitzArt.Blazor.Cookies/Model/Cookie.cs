namespace BitzArt.Blazor.Cookies;

public class Cookie
{
    public string Key { get; set; }
    public string Value { get; set; }
    public DateTimeOffset? Expiration { get; set; }

    public Cookie(string key, string value, DateTimeOffset? expiration = null)
    {
        Key = key;
        Value = value;
        Expiration = expiration;
    }
}
