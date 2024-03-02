using Microsoft.JSInterop;

namespace BitzArt.Blazor.Cookies;

internal class CookieService(IJSRuntime js) : ICookieService
{
    public async Task<IEnumerable<Cookie>> GetAllAsync()
    {
        var raw = await js.InvokeAsync<string>("eval", "document.cookie");
        if (string.IsNullOrWhiteSpace(raw)) return Enumerable.Empty<Cookie>();

        return raw.Split("; ").Select(x =>
        {
            var parts = x.Split("=");
            return new Cookie(parts[0], parts[1]);
        });
    }

    public async Task<Cookie?> GetAsync(string key)
    {
        var cookies = await GetAllAsync();
        return cookies.FirstOrDefault(x => x.Key == key);
    }

    public async Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default)
    {
        if (!cookie.Expiration.HasValue) throw new Exception("Expiration is required when setting cookie.");
        await SetAsync(cookie.Key, cookie.Value, cookie.Expiration!.Value, cancellationToken);
    }

    public async Task SetAsync(string key, string value, DateTimeOffset expiration, CancellationToken cancellationToken = default)
    {
        await js.InvokeVoidAsync("eval", $"document.cookie = \"{key}={value}; expires={expiration:R}; path=/\"");
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new Exception("Key is required when removing cookie.");

        await js.InvokeVoidAsync("eval", $"document.cookie = \"{key}=; expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/\"");
    }
}
