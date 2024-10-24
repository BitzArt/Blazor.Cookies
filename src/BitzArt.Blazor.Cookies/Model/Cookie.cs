namespace BitzArt.Blazor.Cookies;

public class Cookie(string key, string value, DateTimeOffset? expiration = null)
{
    public string Key { get; set; } = key;
    public string Value { get; set; } = value;
    public DateTimeOffset? Expiration { get; set; } = expiration;

    public bool Equals(string key, string value, DateTimeOffset? expiration = null)
    {
        if (Key != key) return false;
        if (Value != value) return false;
        if (Expiration != expiration) return false;

        return true;
    }
}
