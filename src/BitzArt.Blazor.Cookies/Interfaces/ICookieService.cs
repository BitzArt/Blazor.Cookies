namespace BitzArt.Blazor.Cookies;

/// <summary>
/// Allows interacting with browser cookies.
/// </summary>
public interface ICookieService
{
    /// <summary>
    /// Retrieves all cookies.
    /// </summary>
    public Task<IEnumerable<Cookie>> GetAllAsync();

    /// <summary>
    /// Retrieves a cookie by its key.
    /// </summary>
    /// <param name="key"> The key of the cookie to retrieve. </param>
    /// <returns> The requested cookie, or null if it does not exist. </returns>
    public Task<Cookie?> GetAsync(string key);

    /// <summary>
    /// Removes a cookie by marking it as expired.
    /// </summary>
    /// <param name="key"> The key of the cookie to remove. </param>
    /// <param name="cancellationToken"> Cancellation token. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="SetAsync(Cookie, CancellationToken)"/>
    /// <param name="key"> The name of the cookie to set. </param>
    /// <param name="value"> The value of the cookie to set. </param>
    /// <param name="expiration"> The cookie's expiration date. </param>
    /// <param name="cancellationToken"> Cancellation token. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public Task SetAsync(string key, string value, DateTimeOffset? expiration, CancellationToken cancellationToken = default)
        => SetAsync(new Cookie(key, value, expiration), cancellationToken);

    /// <summary>
    /// Adds or updates a browser cookie. <br/> <br/>
    /// When in <see href="https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes">Static SSR render mode</see>,
    /// the new value will be sent to the client machine
    /// after the page has completed rendering,
    /// and will not appear in the cookies collection until the next request.
    /// </summary>
    /// <param name="cookie"> The cookie to set. </param>
    /// <param name="cancellationToken"> Cancellation token. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default);
}
