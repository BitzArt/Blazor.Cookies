using Microsoft.Extensions.DependencyInjection;

namespace BitzArt.Blazor.Cookies;

public static class AddBlazorCookiesExtension
{
    public static IServiceCollection AddBlazorCookies(this IServiceCollection services)
    {
        services.AddScoped<ICookieService, CookieService>();
        return services;
    }
}
