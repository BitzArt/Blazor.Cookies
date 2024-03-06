using Microsoft.JSInterop;

namespace BitzArt.Blazor.Cookies;

internal class BrowserCookieService(IJSRuntime js) : ICookieService
{
    public async Task<IEnumerable<Cookie>> GetAllAsync()
    {
        var cookies = await js.InvokeAsync<IEnumerable<JsCookie>>("cookieStore.getAll");
        return cookies.Select(c => c.Parse());
    }

    public async Task<Cookie?> GetAsync(string key)
    {
        var cookie = await js.InvokeAsync<JsCookie>("cookieStore.get", key);
        return cookie?.Parse();
    }

    public async Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default)
    {
        await SetAsync(cookie.Name, cookie.Value, cookie.Expiration, cancellationToken);
    }

    public async Task SetAsync(string key, string value, DateTimeOffset? expiration, CancellationToken cancellationToken = default)
    {
        await js.InvokeVoidAsync("cookieStore.set", new JsCookie(key, value, expiration));
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await js.InvokeVoidAsync("cookieStore.delete", key);
    }
}
