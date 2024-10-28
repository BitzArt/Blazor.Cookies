using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace BitzArt.Blazor.Cookies;

internal class HttpContextCookieService : ICookieService
{
    private readonly HttpContext _httpContext;
    private readonly Dictionary<string, Cookie> _requestCookies;

    private readonly ILogger _logger;

    private IHeaderDictionary _responseHeaders { get; set; }

    public HttpContextCookieService(IHttpContextAccessor httpContextAccessor, IFeatureCollection features, ILogger<ICookieService> logger)
    {
        _httpContext = httpContextAccessor.HttpContext!;
        _logger = logger;

        _requestCookies = _httpContext.Request.Cookies
            .Select(x => new Cookie(x.Key, x.Value)).ToDictionary(cookie => cookie.Key);

        _responseHeaders = features.GetRequiredFeature<IHttpResponseFeature>().Headers;
    }

    public Task<IEnumerable<Cookie>> GetAllAsync()
    {
        return Task.FromResult(_requestCookies.Select(x => x.Value).ToList().AsEnumerable());
    }

    public Task<Cookie?> GetAsync(string key)
    {
        if (_requestCookies.TryGetValue(key, out var cookie)) return Task.FromResult<Cookie?>(cookie);

        return Task.FromResult<Cookie?>(null);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (RemovePending(key)) _logger.LogDebug("Pending cookie [{key}] removed.", key);

        if (_requestCookies.Remove(key))
        {
            _logger.LogDebug("Removing client browser cookie [{key}] by marking it as expired.", key);
            _httpContext.Response.Cookies.Delete(key);
        }

        return Task.CompletedTask;
    }

    public Task SetAsync(string key, string value, DateTimeOffset? expiration, CancellationToken cancellationToken = default)
        => SetAsync(new Cookie(key, value, expiration), cancellationToken);

    public Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Setting cookie: '{key}'='{value}'", cookie.Key, cookie.Value);

        RemovePending(cookie.Key);

        _httpContext.Response.Cookies.Append(cookie.Key, cookie.Value, new CookieOptions
        {
            Expires = cookie.Expiration,
            Path = "/",
        });

        return Task.CompletedTask;
    }

    private bool RemovePending(string key)
    {
        _logger.LogDebug("Checking for pending cookie: '{key}'", key);

        var cookieValues = _responseHeaders
            .SetCookie
            .ToList();

        for (int i = 0; i < cookieValues.Count; i++)
        {
            var value = cookieValues[i];
            if (string.IsNullOrWhiteSpace(value)) continue;
            if (!value.StartsWith($"{key}=")) continue;

            _logger.LogDebug("Pending cookie [{key}] found, removing...", key);
            cookieValues.RemoveAt(i);
            _responseHeaders.SetCookie = new([.. cookieValues]);
            _logger.LogDebug("Pending cookie [{key}] removed.", key);

            return true;
        }

        _logger.LogDebug("No pending cookie found.");
        return false;
    }
}
