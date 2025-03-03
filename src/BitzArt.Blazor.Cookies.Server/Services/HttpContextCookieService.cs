using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BitzArt.Blazor.Cookies.Server;

internal class HttpContextCookieService : ICookieService
{
    private readonly HttpContext _httpContext;
    private readonly Dictionary<string, Cookie> _requestCookies;

    private readonly ILogger _logger;

    private IHeaderDictionary ResponseHeaders { get; set; }

    public HttpContextCookieService(IHttpContextAccessor httpContextAccessor, ILogger<ICookieService> logger)
    {
        _httpContext = httpContextAccessor.HttpContext!;
        _logger = logger;

        _requestCookies = _httpContext.Request.Cookies
            .Select(x => new Cookie(x.Key, x.Value)).ToDictionary(cookie => cookie.Key);

        ResponseHeaders = _httpContext.Features.GetRequiredFeature<IHttpResponseFeature>().Headers;
    }

    // ======================================== GetAllAsync ========================================

    public Task<IEnumerable<Cookie>> GetAllAsync()
    {
        return Task.FromResult(_requestCookies.Select(x => x.Value).ToList().AsEnumerable());
    }

    // ========================================  GetAsync  ========================================

    public Task<Cookie?> GetAsync(string key)
    {
        if (_requestCookies.TryGetValue(key, out var cookie)) return Task.FromResult<Cookie?>(cookie);

        return Task.FromResult<Cookie?>(null);
    }

    public Task<Cookie<T>?> GetAsync<T>(string key, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        if (_requestCookies.TryGetValue(key, out var cookie))
        {
            return Task.FromResult<Cookie<T>?>(cookie.Cast<T>(jsonSerializerOptions));
        }
        return Task.FromResult<Cookie<T>?>(null);
    }

    // ========================================  SetAsync  ========================================

    public Task SetAsync(string key, string value, DateTimeOffset? expiration = null, bool httpOnly = false, bool secure = false, SameSiteMode? sameSiteMode = null, CancellationToken cancellationToken = default)
        => SetAsync(new Cookie(key, value, expiration, httpOnly, secure, sameSiteMode), cancellationToken);

    public Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default)
    {
        if (cookie.Secure && !cookie.HttpOnly) throw new InvalidOperationException("Unable to set a cookie: Secure cookies must also be HttpOnly.");

        _logger.LogDebug("Setting cookie: '{key}'='{value}'", cookie.Key, cookie.Value);

        RemovePending(cookie.Key);

        _httpContext.Response.Cookies.Append(cookie.Key, cookie.Value, new CookieOptions
        {
            Expires = cookie.Expiration,
            Path = "/",
            HttpOnly = cookie.HttpOnly,
            Secure = cookie.Secure,
            SameSite = cookie.SameSiteMode.ToHttp()
        });

        return Task.CompletedTask;
    }

    private bool RemovePending(string key)
    {
        _logger.LogDebug("Checking for pending cookie: '{key}'", key);

        var cookieValues = ResponseHeaders
            .SetCookie
            .ToList();

        for (int i = 0; i < cookieValues.Count; i++)
        {
            var value = cookieValues[i];
            if (string.IsNullOrWhiteSpace(value)) continue;
            if (!value.StartsWith($"{key}=")) continue;

            _logger.LogDebug("Pending cookie [{key}] found, removing...", key);
            cookieValues.RemoveAt(i);
            ResponseHeaders.SetCookie = new([.. cookieValues]);
            _logger.LogDebug("Pending cookie [{key}] removed.", key);

            return true;
        }

        _logger.LogDebug("No pending cookie found.");
        return false;
    }

    // ======================================== RemoveAsync ========================================

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
}
