using Microsoft.JSInterop;
using System.Text.Json;

namespace BitzArt.Blazor.Cookies;

internal class JsInteropCookieService(IJSRuntime js) : ICookieService
{
    // ======================================== GetAllAsync ========================================

    public async Task<IEnumerable<Cookie>> GetAllAsync()
    {
        var raw = await js.InvokeAsync<string>("eval", "document.cookie");
        if (string.IsNullOrWhiteSpace(raw)) return [];

        return raw.Split("; ").Select(GetCookie);
    }

    // ========================================  GetAsync  ========================================

    private Cookie GetCookie(string raw)
    {
        var parts = raw.Split("=", 2);
        return new Cookie(parts[0], parts[1], null, httpOnly: false, secure: false);
    }

    public async Task<Cookie?> GetAsync(string key)
    {
        var cookies = await GetAllAsync();
        return cookies.FirstOrDefault(x => x.Key == key);
    }

    public async Task<Cookie<T>?> GetAsync<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var cookie = await GetAsync(key);
        if (cookie is null) return null;
        return cookie.Cast<T>(jsonSerializerOptions);
    }

    // ========================================  SetAsync  ========================================

    public Task SetAsync(string key, string value, DateTimeOffset? expiration = null, bool httpOnly = false, bool secure = false, SameSiteMode? sameSiteMode = null, CancellationToken cancellationToken = default)
        => SetAsync(new Cookie(key, value, expiration, httpOnly, secure, sameSiteMode), cancellationToken);

    public async Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default)
    {
        if (cookie.HttpOnly) throw new InvalidOperationException(HttpOnlyFlagErrorMessage);
        if (cookie.Secure) throw new InvalidOperationException(SecureFlagErrorMessage);

        if (string.IsNullOrWhiteSpace(cookie.Key)) throw new Exception("Key is required when setting a cookie.");

        var cmd = JsCommand.SetCookie(cookie);

        await js.InvokeVoidAsync("eval", cmd);
    }

    private const string HttpOnlyFlagErrorMessage = $"HttpOnly cookies are not supported in this rendering environment. {CookieFlagsExplainMessage}";
    private const string SecureFlagErrorMessage = $"Secure cookies are not supported in this rendering environment. {CookieFlagsExplainMessage}";
    private const string CookieFlagsExplainMessage = "Setting HttpOnly or Secure cookies is only possible when using Static SSR render mode.";

    // ======================================== RemoveAsync ========================================

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new Exception("Key is required when removing a cookie.");
        await js.InvokeVoidAsync("eval", $"document.cookie = \"{key}=; expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/\"");
    }
}
