using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BitzArt.Blazor.Cookies;

/// <summary>
/// Extension methods for registering client-side <c>Blazor.Cookies</c> services.
/// </summary>
public static class AddBlazorCookiesExtension
{
    /// <summary>
    /// Registers client-side <c>Blazor.Cookies</c> services.
    /// </summary>
    /// <param name="builder">An <see cref="WebAssemblyHostBuilder"/> to add the services to.</param>
    /// <param name="lifetime"><see cref="ServiceLifetime"/> to register the services with.</param>
    /// <returns><see cref="WebAssemblyHostBuilder"/> for method chaining.</returns>
    public static WebAssemblyHostBuilder AddBlazorCookies(this WebAssemblyHostBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        builder.Services.AddBlazorCookiesClientSideServices(lifetime);

        return builder;
    }

    /// <summary>
    /// Registers client-side <c>Blazor.Cookies</c> services.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add the <c>Blazor.Cookies</c> services to.</param>
    /// <param name="lifetime"><see cref="ServiceLifetime"/> to register the services with.</param>
    /// <returns><see cref="IServiceCollection"/> for method chaining.</returns>
    public static IServiceCollection AddBlazorCookiesClientSideServices(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.Add(new ServiceDescriptor(typeof(ICookieService), typeof(JsInteropCookieService), lifetime));

        return services;
    }
}
