using System.Text;
using System.Text.Json;

namespace BitzArt.Blazor.Cookies;

/// <inheritdoc cref="Cookie{T}"/>
public class Cookie
{
    private protected string _value;

    /// <summary>
    /// Name of the cookie.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Value of the cookie.
    /// </summary>
    public string Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged(value);
        }
    }

    /// <summary>
    /// Cookie expiration date.
    /// </summary>
    public DateTimeOffset? Expiration { get; set; }

    /// <summary>
    /// Whether the cookie is inaccessible by client-side script.
    /// </summary>
    public bool HttpOnly { get; set; }

    /// <summary>
    /// Whether to transmit the cookie using Secure Sockets Layer (SSL)--that is, over HTTPS only.
    /// </summary>
    public bool Secure { get; set; }

    /// <summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie#samesitesamesite-value">SameSiteMode</see>
    /// controls whether or not a cookie is sent with cross-site requests, providing some protection against cross-site request forgery attacks
    /// (<see href="https://developer.mozilla.org/en-US/docs/Glossary/CSRF">CSRF</see>). <br />
    /// <b>Note:</b> Null value will result in the browser using it's default behavior.
    /// </summary>
    public SameSiteMode? SameSiteMode { get; set; }

    /// <summary>
    /// Creates a new <see cref="Cookie"/>.
    /// </summary>
    /// <param name="key"> Name of the cookie. </param>
    /// <param name="value"> Value of the cookie. </param>
    /// <param name="expiration"> Cookie expiration date. </param>
    /// <param name="httpOnly"> Whether the cookie is inaccessible by client-side script. </param>
    /// <param name="secure"> Whether to transmit the cookie using Secure Sockets Layer (SSL)--that is, over HTTPS only. </param>
    /// <param name="sameSiteMode">
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie#samesitesamesite-value">SameSiteMode</see>
    /// controls whether or not a cookie is sent with cross-site requests, providing some protection against cross-site request forgery attacks
    /// (<see href="https://developer.mozilla.org/en-US/docs/Glossary/CSRF">CSRF</see>). <br />
    /// <b>Note:</b> Null value will result in the browser using it's default behavior.
    /// </param>
    public Cookie(string key, string value, DateTimeOffset? expiration = null, bool httpOnly = false, bool secure = false, SameSiteMode? sameSiteMode = null)
    {
        Key = key;
        _value = value;
        Expiration = expiration;
        HttpOnly = httpOnly;
        Secure = secure;
        SameSiteMode = sameSiteMode;
    }

    private protected virtual void OnValueChanged(string value) { }

    /// <inheritdoc cref="Cookie(string, string, DateTimeOffset?, bool, bool, SameSiteMode?)"/>
    public static Cookie<T> FromValue<T>(string key, T value, DateTimeOffset? expiration = null, bool httpOnly = false, bool secure = false, SameSiteMode? sameSiteMode = null, JsonSerializerOptions? jsonSerializerOptions = null)
        => new(key, value, expiration, httpOnly, secure, sameSiteMode, jsonSerializerOptions);

    /// <summary>
    /// Casts a <see cref="Cookie"/> to a <see cref="Cookie{T}"/>.
    /// </summary>
    /// <typeparam name="T">Target type of the cookie value.</typeparam>
    /// <param name="jsonSerializerOptions">JSON serializer options to use when deserializing the value.</param>
    /// <returns><see cref="Cookie{T}"/>, where T: <typeparamref name="T"/>.</returns>
    public Cookie<T> Cast<T>(JsonSerializerOptions? jsonSerializerOptions = null)
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new InvalidOperationException("Cannot cast a cookie with an empty value. Ensure the cookie has been set before casting.");
        }
        return new(Key, Cookie<T>.DecodeValue(Value, jsonSerializerOptions ?? new())!, Expiration, HttpOnly, Secure, SameSiteMode, jsonSerializerOptions);
    }
}

/// <summary>
/// Browser cookie.
/// <para>
/// <b>Note:</b> When retrieving a cookie, certain properties of the resulting cookie object may be unavailable. 
/// This is because browsers do not expose these attributes of cookies to neither client-side or server-side code.
/// Only the cookie's key and value are accessible, with the browser keeping other attributes
/// (such as `HttpOnly`, `Secure`, and `SameSite`) hidden for security and privacy reasons.
/// </para>
/// </summary>
/// <typeparam name="T"> Type of the cookie value. </typeparam>
public class Cookie<T> : Cookie
{
    private protected new T? _value;

    /// <summary>
    /// The value of the cookie.
    /// </summary>
    public new T? Value
    {
        get => _value;
        set
        {
            _value = value;
            base._value = EncodeValue(value, JsonSerializerOptions);
        }
    }

    private protected override void OnValueChanged(string value)
    {
        _value = DecodeValue(base.Value, JsonSerializerOptions);
    }

    /// <summary>
    /// JSON serialization options to use when serializing/deserializing the cookie value.
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();

    /// <inheritdoc cref="Cookie(string, string, DateTimeOffset?, bool, bool, SameSiteMode?)"/>
    public Cookie(string key, T value, DateTimeOffset? expiration = null, bool httpOnly = false, bool secure = false, SameSiteMode? sameSiteMode = null, JsonSerializerOptions? jsonSerializerOptions = null)
        : base(key, EncodeValue(value, jsonSerializerOptions ?? new()), expiration, httpOnly, secure, sameSiteMode)
    {
        _value = value;
        JsonSerializerOptions = jsonSerializerOptions ?? new();
    }

    internal static string EncodeValue(T? value, JsonSerializerOptions jsonSerializerOptions)
    {
        if (value is null) return string.Empty;

        var json = JsonSerializer.Serialize(value, jsonSerializerOptions);
        var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        return base64Encoded;
    }

    internal static T? DecodeValue(string value, JsonSerializerOptions jsonSerializerOptions)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return (T?)(object?)null;
        }
        var base64Decoded = Convert.FromBase64String(value);
        var json = Encoding.UTF8.GetString(base64Decoded);
        var deserialized = JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
        return deserialized;
    }
}
