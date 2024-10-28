using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BitzArt.Blazor.Cookies;

public static class AddBlazorCookiesExtension
{
    public static WebAssemblyHostBuilder AddBlazorCookies(this WebAssemblyHostBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        builder.Services.AddBlazorCookiesClient(lifetime);

        return builder;
    }

    private static IServiceCollection AddBlazorCookiesClient(this IServiceCollection services, ServiceLifetime lifetime)
    {
        services.Add(new ServiceDescriptor(typeof(ICookieService), typeof(JsInteropCookieService), lifetime));

        return services;
    }
}
