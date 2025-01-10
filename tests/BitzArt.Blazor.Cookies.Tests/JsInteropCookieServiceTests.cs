using Microsoft.JSInterop;
using Moq;

namespace BitzArt.Blazor.Cookies.Tests;

public class JsInteropCookieServiceTests
{
    [Fact]
    public async Task GetAsync_WithCookieNotPresent_ShouldReturnNull()
    {
        // Arrange
        var jsRuntime = new Mock<IJSRuntime>();
        jsRuntime.Setup(x => x.InvokeAsync<string>("eval", It.IsAny<object[]>()))
            .ReturnsAsync("");

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
        var jsRuntime = new Mock<IJSRuntime>();
        jsRuntime.Setup(x => x.InvokeAsync<string>("eval", It.IsAny<object[]>()))
            .ReturnsAsync($"{cookieName}={cookieValue}; path=/");

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
}