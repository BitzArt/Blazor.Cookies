namespace BitzArt.Blazor.Cookies;

public record Cookie(string Key, string Value, DateTimeOffset? Expiration = null) { }
