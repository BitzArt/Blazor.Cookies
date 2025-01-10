namespace BitzArt.Blazor.Cookies;

/// <summary>
/// Browser cookie.
/// <para>
/// <b>Note:</b> When retrieving a cookie, certain properties of the resulting cookie object may be unavailable. 
/// This is because browsers do not expose these attributes of cookies to neither client-side or server-side code.
/// Only the cookie's key and value are accessible, with the browser keeping other attributes
/// (such as `HttpOnly`, `Secure`, and `SameSite`) hidden for security and privacy reasons.
/// </para>
/// </summary>
/// <param name="Key"> The name of the cookie. </param>
/// <param name="Value"> The value of the cookie. </param>
/// <param name="Expiration"> The expiration date of the cookie. </param>
/// <param name="HttpOnly"> Whether the cookie is inaccessible by client-side script. </param>
/// <param name="Secure"> Whether to transmit the cookie using Secure Sockets Layer (SSL)--that is, over HTTPS only. </param>
/// <param name="SameSiteMode">
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie#samesitesamesite-value">SameSiteMode</see>
/// controls whether or not a cookie is sent with cross-site requests, providing some protection against cross-site request forgery attacks
/// (<see href="https://developer.mozilla.org/en-US/docs/Glossary/CSRF">CSRF</see>). <br />
/// <b>Note:</b> Null value will result in the browser using it's default behavior.
/// </param>
public record Cookie(string Key, string Value, DateTimeOffset? Expiration = null, bool HttpOnly = false, bool Secure = false, SameSiteMode? SameSiteMode = null);
