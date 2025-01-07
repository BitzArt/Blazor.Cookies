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
        await service.SetAsync("key", "value");

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

        await service.SetAsync("key", "value");

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
        await service.SetAsync("key", "value1");
        await service.SetAsync("key", "value2");

        // Assert
        var values = httpContext.Features.GetRequiredFeature<IHttpResponseFeature>().Headers.SetCookie;
        Assert.Single(values);
    }

    [Fact]
    public async Task SetCookie_WithHttpOnlyFlag_ShouldSetHttpOnly()
    {
        // Arrange
        (var httpContext, _, var service) = CreateTestServices();

        // Act
        await service.SetAsync("key", "value", httpOnly: true, secure: false);

        // Assert
        var values = httpContext.Features.GetRequiredFeature<IHttpResponseFeature>().Headers.SetCookie;
        Assert.Single(values);
        Assert.Contains("httponly", values.First());
    }

    [Fact]
    public async Task SetCookie_WithSecureFlagButNotHttpOnlyFlag_ShouldThrow()
    {
        // Arrange
        (var httpContext, _, var service) = CreateTestServices();

        // Act + Assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await service.SetAsync("key", "value", httpOnly: false, secure: true));
    }

    [Fact]
    public async Task SetCookie_WithHttpOnlyAndSecureFlags_ShouldSetHttpOnlyAndSecure()
    {
        // Arrange
        (var httpContext, _, var service) = CreateTestServices();

        // Act
        await service.SetAsync("key", "value", httpOnly: true, secure: true);

        // Assert
        var values = httpContext.Features.GetRequiredFeature<IHttpResponseFeature>().Headers.SetCookie;
        Assert.Single(values);
        Assert.Contains("httponly", values.First());
        Assert.Contains("secure", values.First());
    }

    private static TestServices CreateTestServices()
    {
        var httpContext = new DefaultHttpContext();
        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var logger = new LoggerFactory().CreateLogger<ICookieService>();

        var cookieService = new HttpContextCookieService(accessor, logger);

        return new TestServices(httpContext, accessor, cookieService);
    }

    private record TestServices(HttpContext HttpContext, IHttpContextAccessor HttpContextAccessor, ICookieService CookieService);
}