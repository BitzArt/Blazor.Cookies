using BitzArt.Blazor.Cookies;
using Microsoft.JSInterop;
using Moq;

namespace BitzArt.Blazor.Auth
{
    public class GetAsyncTests
    {
        [Fact]
        public async Task GetAsyncShouldHandleValue()
        {
            var jsRuntime = new Mock<IJSRuntime>();
            jsRuntime.Setup(x => x.InvokeAsync<string>("eval", It.IsAny<object[]>()))
                .ReturnsAsync("token=value123; path=/");
            var result = await new JsInteropCookieService(jsRuntime.Object).GetAsync("token");
            Assert.Equal("value123", result?.Value);
        }        
        
        [Fact]
        public async Task GetAsyncShouldHandleEqualsInValue()
        {
            var jsRuntime = new Mock<IJSRuntime>();
            jsRuntime.Setup(x => x.InvokeAsync<string>("eval", It.IsAny<object[]>()))
                .ReturnsAsync("token=value=123; path=/");
            var result = await new JsInteropCookieService(jsRuntime.Object).GetAsync("token");
            Assert.Equal("value=123", result?.Value);
        }
    }
}