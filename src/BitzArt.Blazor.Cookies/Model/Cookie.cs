namespace BitzArt.Blazor.Cookies;

/// <summary>
/// Browser cookie.
/// </summary>
/// <param name="Key"> The name of the cookie. </param>
/// <param name="Value"> The value of the cookie. </param>
/// <param name="Expiration"> The expiration date of the cookie. </param>
public record Cookie(string Key, string Value, DateTimeOffset? Expiration = null) { }
