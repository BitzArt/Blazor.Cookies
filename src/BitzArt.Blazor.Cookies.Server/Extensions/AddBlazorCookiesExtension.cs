using BitzArt.Blazor.Cookies.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BitzArt.Blazor.Cookies;

public static class AddBlazorCookiesExtension
{
    public static IHostApplicationBuilder AddBlazorCookies(this IHostApplicationBuilder builder)
    {
        builder.Services.AddBlazorCookiesServerSideServices();
        return builder;
    }

    public static IServiceCollection AddBlazorCookiesServerSideServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<JsInteropCookieService>();
        services.AddScoped<HttpContextCookieService>();

        services.AddScoped<ICookieService>(x =>
        {
            var httpContextAccessor = x.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;
            var isPrerendering = (httpContext is not null && !httpContext.Response.HasStarted);

            return isPrerendering
                ? x.GetRequiredService<HttpContextCookieService>()
                : x.GetRequiredService<JsInteropCookieService>();
        });

        return services;
    }
}
