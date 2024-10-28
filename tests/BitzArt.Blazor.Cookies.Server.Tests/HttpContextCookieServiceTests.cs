using Microsoft.AspNetCore.Http;

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
        Assert.Single(httpContext.Response.Headers["Set-Cookie"]);
        Assert.Contains("key=value", httpContext.Response.Headers["Set-Cookie"].First());
    }

    [Fact]
    public async Task RemoveCookie_AfterSetCookie_ShouldRemoveCookie()
    {
        // Arrange
        (var httpContext, _, var service) = CreateTestServices();

        await service.SetAsync("key", "value", null);

        // Act
        await service.RemoveAsync("key");

        // Assert
        Assert.Single(httpContext.Response.Headers);
        Assert.Single(httpContext.Response.Headers["Set-Cookie"]);
        Assert.Contains("key=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=", httpContext.Response.Headers["Set-Cookie"].First());
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
        Assert.Single(httpContext.Response.Headers);
    }

    private static TestServices CreateTestServices()
    {
        var httpContext = new DefaultHttpContext();
        var accessor = new TestHttpContextAccessor(httpContext);

        var cookieService = new HttpContextCookieService(accessor);

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