using Microsoft.JSInterop;
using System.Text.Json;
using System.Web;

namespace BitzArt.Blazor.Cookies;

internal class JsInteropCookieService(IJSRuntime js) : ICookieService, IAsyncDisposable
{
    private readonly SemaphoreSlim _moduleLock = new(1, 1);
    private IJSObjectReference? _module;
    private bool _disposed;

    // ======================================== GetAllAsync ========================================

    public async Task<IEnumerable<Cookie>> GetAllAsync()
    {
        var module = await GetModuleAsync();
        var raw = await module.InvokeAsync<string>("getCookies");
        if (string.IsNullOrWhiteSpace(raw)) return [];

        return raw.Split("; ").Select(GetCookie);
    }

    private Cookie GetCookie(string raw)
    {
        var decoded = HttpUtility.UrlDecode(raw);
        var parts = decoded.Split("=", 2);

        return new Cookie(parts[0], parts[1], null, httpOnly: false, secure: false);
    }

    // ========================================  GetAsync  ========================================

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

    public async Task<Cookie<T>> GetAsync<T>(string key, Func<T> getDefaultValue, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        try
        {
            var cookie = await GetAsync(key);
            if (cookie is null)
            {
                return Cookie.FromValue(key, getDefaultValue.Invoke());
            }
            if (string.IsNullOrWhiteSpace(cookie.Value))
            {
                return new Cookie<T>(key, ((T?)(object?)null)!);
            }
            return cookie.Cast<T>(jsonSerializerOptions);
        }
        catch
        {
            return Cookie.FromValue(key, getDefaultValue.Invoke());
        }
        
    }

    public async Task<T?> GetValueAsync<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var cookie = await GetAsync<T>(key);
        if (cookie is null) return (T?)(object?)null;
        return cookie.Value;
    }

    public async Task<T> GetValueAsync<T>(string key, Func<T> getDefault, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        try
        {
            var cookie = await GetAsync<T>(key);
            if (cookie is null)
            {
                return getDefault.Invoke();
            }
            if (cookie.Value is null)
            {
                return getDefault.Invoke();
            }

            return cookie.Value;
        }
        catch
        {
            return getDefault.Invoke();
        }
    }

    // ========================================  SetAsync  ========================================

    public Task SetAsync(string key, string value, DateTimeOffset? expiration = null, bool httpOnly = false, bool secure = false, SameSiteMode? sameSiteMode = null, CancellationToken cancellationToken = default)
        => SetAsync(new Cookie(key, value, expiration, httpOnly, secure, sameSiteMode), cancellationToken);

    public async Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default)
    {
        if (cookie.HttpOnly) throw new InvalidOperationException(HttpOnlyFlagErrorMessage);
        if (cookie.Secure) throw new InvalidOperationException(SecureFlagErrorMessage);

        if (string.IsNullOrWhiteSpace(cookie.Key)) throw new Exception("Key is required when setting a cookie.");

        var cookieString = CookieStringBuilder.Build(cookie);
        var module = await GetModuleAsync(cancellationToken);

        await module.InvokeVoidAsync("setCookie", cancellationToken, cookieString);
    }

    private const string HttpOnlyFlagErrorMessage = $"HttpOnly cookies are not supported in this rendering environment. {CookieFlagsExplainMessage}";
    private const string SecureFlagErrorMessage = $"Secure cookies are not supported in this rendering environment. {CookieFlagsExplainMessage}";
    private const string CookieFlagsExplainMessage = "Setting HttpOnly or Secure cookies is only possible when using Static SSR render mode.";

    // ======================================== RemoveAsync ========================================

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new Exception("Key is required when removing a cookie.");

        var keyEncoded = HttpUtility.UrlEncode(key);
        var cookieString = $"{keyEncoded}=; expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/";
        var module = await GetModuleAsync(cancellationToken);

        await module.InvokeVoidAsync("setCookie", cancellationToken, cookieString);
    }

    private async Task<IJSObjectReference> GetModuleAsync(CancellationToken cancellationToken = default)
    {
        await _moduleLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            _module ??= await js.InvokeAsync<IJSObjectReference>(
                "import", cancellationToken, "./_content/BitzArt.Blazor.Cookies/cookies.js");

            return _module;
        }
        finally
        {
            _moduleLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleLock.WaitAsync();
        try
        {
            if (_disposed) return;
            _disposed = true;

            if (_module is not null)
            {
                try
                {
                    await _module.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                    // The browser reference cannot be released after the circuit disconnects.
                }
            }
        }
        finally
        {
            _moduleLock.Release();
        }
    }
}
