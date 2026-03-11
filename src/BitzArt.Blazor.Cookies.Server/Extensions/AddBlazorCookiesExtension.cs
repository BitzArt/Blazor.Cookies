using BitzArt.Blazor.Cookies.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BitzArt.Blazor.Cookies;

/// <summary>
/// Extension methods for registering server-side <c>Blazor.Cookies</c> services.
/// </summary>
public static class AddBlazorCookiesExtension
{
    /// <summary>
    /// Registers server-side <c>Blazor.Cookies</c> services.
    /// </summary>
    /// <param name="builder">An <see cref="IHostApplicationBuilder"/> to add the services to.</param>
    /// <returns><see cref="IHostApplicationBuilder"/> for method chaining. </returns>
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
