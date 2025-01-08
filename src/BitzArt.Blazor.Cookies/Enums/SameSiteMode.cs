namespace BitzArt.Blazor.Cookies;

/// <summary>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie#samesitesamesite-value">SameSiteMode</see>
/// controls whether or not a cookie is sent with cross-site requests, providing some protection against cross-site request forgery attacks
/// (<see href="https://developer.mozilla.org/en-US/docs/Glossary/CSRF">CSRF</see>).
/// </summary>
public enum SameSiteMode
{
	/// <summary>
	/// Indicates the client should disable same-site restrictions.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the client should send the cookie with "same-site" requests, and with
	/// "cross-site" top-level navigations.
	/// </summary>
	Lax = 1,

	/// <summary>
	/// Indicates the client should only send the cookie with "same-site" requests.
	/// </summary>
	Strict = 2
}
