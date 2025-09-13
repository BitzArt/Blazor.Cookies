using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BitzArt.Blazor.Cookies;

public static class AddBlazorCookiesExtension
{
    public static WebAssemblyHostBuilder AddBlazorCookies(this WebAssemblyHostBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        builder.Services.AddBlazorCookiesClientSideServices(lifetime);

        return builder;
    }

    public static IServiceCollection AddBlazorCookiesClientSideServices(this IServiceCollection services, ServiceLifetime lifetime)
    {
        services.Add(new ServiceDescriptor(typeof(ICookieService), typeof(JsInteropCookieService), lifetime));

        return services;
    }
}
