namespace BitzArt.Blazor.Cookies.Server;

internal static class SameSiteModeExtensions
{
    /// <summary>
    /// Converts BitzArt.Blazor.Cookies.SameSiteMode values to Microsoft.AspNetCore.Http.SameSiteMode values
    /// </summary>
    public static Microsoft.AspNetCore.Http.SameSiteMode ToHttp(this BitzArt.Blazor.Cookies.SameSiteMode? sameSiteMode)
        => sameSiteMode switch
        {
            SameSiteMode.None => Microsoft.AspNetCore.Http.SameSiteMode.None,
            SameSiteMode.Lax => Microsoft.AspNetCore.Http.SameSiteMode.Lax,
            SameSiteMode.Strict => Microsoft.AspNetCore.Http.SameSiteMode.Strict,
            null => Microsoft.AspNetCore.Http.SameSiteMode.Unspecified,

            _ => throw new ArgumentOutOfRangeException(nameof(sameSiteMode), sameSiteMode, null)
        };
}
