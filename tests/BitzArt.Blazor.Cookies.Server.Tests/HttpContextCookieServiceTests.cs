using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace BitzArt.Blazor.Cookies.Server.Tests;

public class HttpContextCookieServiceTests
{
    [Fact]
    public async Task SetCookie_WhenProperCookie_ShouldSetCookie()
    {
        // Arrange
        (var httpContext, _, var service) = CreateTestServices();

        // Act
        await service.SetAsync("key", "value", null);

        // Assert
        Assert.Single(httpContext.Response.Headers);
        var values = httpContext.Response.Headers.SetCookie;
        Assert.Single(values);
        var value = values.First();
        Assert.Equal("key=value; path=/", value);
    }

    [Fact]
    public async Task RemoveCookie_AfterSetCookie_ShouldRemovePending()
    {
        // Arrange
        (var httpContext, _, var service) = CreateTestServices();

        await service.SetAsync("key", "value", null);

        // Act
        await service.RemoveAsync("key");

        // Assert
        Assert.Empty(httpContext.Response.Headers);
        Assert.True(httpContext.Response.Headers.SetCookie.Count == 0);
    }

    [Fact]
    public async Task SetCookie_WhenDuplicate_ShouldOnlySetCookieOnce()
    {
        // Arrange
        (var httpContext, _, var service) = CreateTestServices();

        // Act
        await service.SetAsync("key", "value1", null);
        await service.SetAsync("key", "value2", null);

        // Assert
        var values = httpContext.Features.GetRequiredFeature<IHttpResponseFeature>().Headers.SetCookie;
        Assert.Single(values);
    }

    private static TestServices CreateTestServices()
    {
        var httpContext = new DefaultHttpContext();
        var accessor = new TestHttpContextAccessor(httpContext);
        var logger = new LoggerFactory().CreateLogger<ICookieService>();

        var cookieService = new HttpContextCookieService(accessor, httpContext.Features, logger);

        return new TestServices(httpContext, accessor, cookieService);
    }

    private record TestServices(HttpContext HttpContext, IHttpContextAccessor HttpContextAccessor, ICookieService CookieService);

    private class TestHttpContextAccessor(HttpContext httpContext) : IHttpContextAccessor
    {
        private HttpContext? _httpContext = httpContext;

        public HttpContext? HttpContext
        {
            get => _httpContext;
            set => _httpContext = value;
        }
    }
}