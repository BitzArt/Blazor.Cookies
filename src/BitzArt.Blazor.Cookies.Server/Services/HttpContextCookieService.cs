using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;

namespace BitzArt.Blazor.Cookies;

internal class HttpContextCookieService : ICookieService
{
    private readonly HttpContext _httpContext;
    private readonly Dictionary<string, Cookie> _cache;

    public HttpContextCookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext!;
        _cache = _httpContext.Request.Cookies
            .Select(x => new Cookie(x.Key, x.Value)).ToDictionary(cookie => cookie.Key);
    }

    public Task<IEnumerable<Cookie>> GetAllAsync()
    {
        return Task.FromResult(_cache.Select(x => x.Value).ToList().AsEnumerable());
    }

    public Task<Cookie?> GetAsync(string key)
    {
        if (_cache.TryGetValue(key, out var cookie)) return Task.FromResult<Cookie?>(cookie);

        return Task.FromResult<Cookie?>(null);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_cache.TryGetValue(key, out _)) return Task.CompletedTask;

        _cache.Remove(key);
        _httpContext.Response.Cookies.Delete(key);

        return Task.CompletedTask;
    }

    public Task SetAsync(string key, string value, DateTimeOffset? expiration, CancellationToken cancellationToken = default)
        => SetAsync(new Cookie(key, value, expiration), cancellationToken);

    public async Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default)
    {
        var alreadyExists = _cache.TryGetValue(cookie.Key, out var existingCookie);

        if (alreadyExists)
        {
            // If the cookie already exists and the value has not changed,
            // we don't need to update it.
            if (existingCookie == cookie) return;

            // If the cookie already exists and the new value has changed,
            // we remove the old one before adding the new one.
            await RemoveAsync(cookie.Key, cancellationToken);
        }

        _cache.Add(cookie.Key, cookie);
        _httpContext.Response.Cookies.Append(cookie.Key, cookie.Value, new CookieOptions
        {
            Expires = cookie.Expiration,
            Path = "/",
        });
    }
}
