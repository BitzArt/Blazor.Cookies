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
    /// <param name="cancellationToken"> Cancellation token. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public Task SetAsync(string key, string value, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="SetAsync(Cookie, CancellationToken)"/>
    /// <param name="key"> The name of the cookie to set. </param>
    /// <param name="value"> The value of the cookie to set. </param>
    /// <param name="expiration"> The cookie's expiration date. </param>
    /// <param name="cancellationToken"> Cancellation token. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public Task SetAsync(string key, string value, DateTimeOffset? expiration, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="SetAsync(Cookie, CancellationToken)"/>
    /// <param name="key"> The name of the cookie to set. </param>
    /// <param name="value"> The value of the cookie to set. </param>
    /// <param name="httpOnly"> Whether the cookie is inaccessible by client-side script. </param>
    /// <param name="secure"> Whether to transmit the cookie using Secure Sockets Layer (SSL)--that is, over HTTPS only. </param>
    /// <param name="cancellationToken"> Cancellation token. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public Task SetAsync(string key, string value, bool httpOnly, bool secure, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="SetAsync(Cookie, CancellationToken)"/>
    /// /// <param name="key"> The name of the cookie to set. </param>
    /// <param name="value"> The value of the cookie to set. </param>
    /// <param name="expiration"> The cookie's expiration date. </param>
    /// <param name="httpOnly"> Whether the cookie is inaccessible by client-side script. </param>
    /// <param name="secure"> Whether to transmit the cookie using Secure Sockets Layer (SSL)--that is, over HTTPS only. </param>
    /// <param name="cancellationToken"> Cancellation token. </param>
    public Task SetAsync(string key, string value, DateTimeOffset? expiration, bool httpOnly, bool secure, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates a browser cookie. <br/> <br/>
    /// <b>Note: </b>
    /// When in <see href="https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes">Static SSR render mode</see>,
    /// the new value will only be sent to the client machine after the page has completed rendering,
    /// and thus will not appear in the cookies collection until the next request.
    /// </summary>
    /// <param name="cookie"> The cookie to set. </param>
    /// <param name="cancellationToken"> Cancellation token. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    public Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default);
}
