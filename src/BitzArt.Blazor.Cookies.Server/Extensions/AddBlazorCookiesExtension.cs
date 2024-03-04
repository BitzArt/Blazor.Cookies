using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BitzArt.Blazor.Cookies;

public static class AddBlazorCookiesExtension
{
    public static IHostApplicationBuilder AddBlazorCookies(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<BrowserCookieService>();
        builder.Services.AddScoped<HttpContextCookieService>();

        builder.Services.AddScoped<ICookieService>(x =>
        {
            var httpContextAccessor = x.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var isPrerendering = (httpContext is not null && !httpContext.Response.HasStarted);

            return isPrerendering
                ? x.GetRequiredService<HttpContextCookieService>()
                : x.GetRequiredService<BrowserCookieService>();
        });

        return builder;
    }
}
