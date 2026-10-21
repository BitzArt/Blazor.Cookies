using Microsoft.JSInterop;
using Moq;

namespace BitzArt.Blazor.Cookies.Tests;

public class JsInteropCookieServiceTests
{
    [Fact]
    public async Task GetAsync_WithCookieNotPresent_ShouldReturnNull()
    {
        // Arrange
        var module = new Mock<IJSObjectReference>();
        module.Setup(x => x.InvokeAsync<string>("getCookies", It.IsAny<object[]>()))
            .ReturnsAsync("");

        var jsRuntime = new Mock<IJSRuntime>();
        jsRuntime.Setup(x => x.InvokeAsync<IJSObjectReference>(
            "import", It.IsAny<CancellationToken>(),
            It.Is<object[]>(args => args.Length == 1 && (string)args[0] == "./_content/BitzArt.Blazor.Cookies/cookies.js")))
            .ReturnsAsync(module.Object);

        var sut = new JsInteropCookieService(jsRuntime.Object);

        // Act
        var result = await sut.GetAsync("my-cookie");

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("my-cookie", "")]
    [InlineData("my-cookie", "my-value")]
    [InlineData("my-cookie", "value=123")]
    [InlineData("my-cookie", "value_a=a value_b=b")]
    [InlineData("my-cookie", "!@#$%^&*()-_=_-)(*&^%$#@!")]
    [InlineData("my-cookie", "abc !@#$%^&*()-_ = _-)(*&^%$#@! def 123456789 ghi")]
    public async Task GetAsync_WithCookiePresent_ShouldReturnValue(string cookieName, string cookieValue)
    {
        // Arrange
        var module = new Mock<IJSObjectReference>();
        module.Setup(x => x.InvokeAsync<string>("getCookies", It.IsAny<object[]>()))
            .ReturnsAsync($"{cookieName}={cookieValue}; path=/");

        var jsRuntime = new Mock<IJSRuntime>();
        jsRuntime.Setup(x => x.InvokeAsync<IJSObjectReference>(
            "import", It.IsAny<CancellationToken>(),
            It.Is<object[]>(args => args.Length == 1 && (string)args[0] == "./_content/BitzArt.Blazor.Cookies/cookies.js")))
            .ReturnsAsync(module.Object);

        var sut = new JsInteropCookieService(jsRuntime.Object);

        // Act
        var result = await sut.GetAsync(cookieName);

        // Assert
        Assert.Equal(cookieValue, result?.Value);
    }

    [Fact]
    public async Task SetAsync_WithHttpOnlyTrue_ShouldThrow()
    {
        // Arrange
        var jsRuntime = new Mock<IJSRuntime>();

        var sut = new JsInteropCookieService(jsRuntime.Object);

        // Act + Assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await sut.SetAsync("cookie-name", "cookie-value", httpOnly: true, secure: false));
    }

    [Fact]
    public async Task SetAsync_WithSecureTrue_ShouldThrow()
    {
        // Arrange
        var jsRuntime = new Mock<IJSRuntime>();

        var sut = new JsInteropCookieService(jsRuntime.Object);

        // Act + Assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await sut.SetAsync("cookie-name", "cookie-value", httpOnly: false, secure: true));
    }

    [Theory]
    [InlineData("my-cookie", "my-cookie")]
    [InlineData("my cookie", "my+cookie")]
    [InlineData("name=value;path=/", "name%3dvalue%3bpath%3d%2f")]
    [InlineData("\";alert(1);//", "%22%3balert(1)%3b%2f%2f")]
    [InlineData("a'b", "a%27b")]
    [InlineData("a\\b", "a%5cb")]
    [InlineData("line\nbreak", "line%0abreak")]
    [InlineData("%22", "%2522")]
    [InlineData("café", "caf%c3%a9")]
    public async Task RemoveAsync_WithCookieName_ShouldPassEncodedNameToModule(string key, string expectedEncodedKey)
    {
        // Arrange
        var expectedCookieString = $"{expectedEncodedKey}=; expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/";
        var cancellationToken = TestContext.Current.CancellationToken;
        var module = new Mock<IJSObjectReference>();
        var jsRuntime = new Mock<IJSRuntime>(MockBehavior.Strict);
        jsRuntime.Setup(x => x.InvokeAsync<IJSObjectReference>(
            "import", cancellationToken,
            It.Is<object[]>(args => args.Length == 1 && (string)args[0] == "./_content/BitzArt.Blazor.Cookies/cookies.js")))
            .ReturnsAsync(module.Object);

        var sut = new JsInteropCookieService(jsRuntime.Object);

        // Act
        await sut.RemoveAsync(key, cancellationToken);

        // Assert
        module.Verify(x => x.InvokeAsync<It.IsAnyType>(
            "setCookie", cancellationToken,
            It.Is<object[]>(args => args.Length == 1 && (string)args[0] == expectedCookieString)),
            Times.Once);
        module.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SetAsync_WithCookie_ShouldPassEncodedCookieStringToModule()
    {
        // Arrange
        var expiration = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var cookie = new Cookie("my cookie", "\";alert(1);//", expiration, sameSiteMode: SameSiteMode.Lax);
        const string expectedCookieString = "my+cookie=%22%3balert(1)%3b%2f%2f; expires=Tue, 01 Jan 2030 00:00:00 GMT; path=/; SameSite=Lax";
        var cancellationToken = TestContext.Current.CancellationToken;
        var module = new Mock<IJSObjectReference>();
        var jsRuntime = new Mock<IJSRuntime>(MockBehavior.Strict);
        jsRuntime.Setup(x => x.InvokeAsync<IJSObjectReference>(
            "import", cancellationToken,
            It.Is<object[]>(args => args.Length == 1 && (string)args[0] == "./_content/BitzArt.Blazor.Cookies/cookies.js")))
            .ReturnsAsync(module.Object);

        var sut = new JsInteropCookieService(jsRuntime.Object);

        // Act
        await sut.SetAsync(cookie, cancellationToken);

        // Assert
        module.Verify(x => x.InvokeAsync<It.IsAnyType>(
            "setCookie", cancellationToken,
            It.Is<object[]>(args => args.Length == 1 && (string)args[0] == expectedCookieString)),
            Times.Once);
        module.VerifyNoOtherCalls();
    }
}
