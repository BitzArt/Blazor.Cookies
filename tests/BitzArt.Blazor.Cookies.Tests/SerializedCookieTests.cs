using System.Text;
using System.Text.Json;

namespace BitzArt.Blazor.Cookies;

public class SerializedCookieTests
{
    [Fact]
    public void Ctor_WithValue_ShouldSaveAndSerializeValue()
    {
        // Arrange
        var value = new { a = "a" };

        // Act
        var cookie = Cookie.FromValue("test-cookie", value);

        // Assert
        Assert.Equal("test-cookie", cookie.Key);

        var json = JsonSerializer.Serialize(value);
        var expectedStringValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        var stringValue = (cookie as Cookie).Value;

        Assert.Equal(expectedStringValue, stringValue);
    }

    [Fact]
    public void ValueSet_WithValue_ShouldSaveAndSerializeValue()
    {
        // Arrange
        var initialValue = new TestPayload("initial value");
        var cookie = Cookie.FromValue("test-cookie", initialValue);

        // Act
        var newValue = new TestPayload("new value");
        cookie.Value = newValue;

        // Assert
        Assert.NotEqual(initialValue, newValue);

        Assert.NotEqual(initialValue, cookie.Value);
        Assert.Equal(newValue, cookie.Value);

        var initialValueSerialized = JsonSerializer.Serialize(initialValue);
        var initialValueEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(initialValueSerialized));

        var expectedValueSerialized = JsonSerializer.Serialize(newValue);
        var expectedValueEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(expectedValueSerialized));

        var stringValue = (cookie as Cookie).Value;

        Assert.NotEqual(initialValueEncoded, stringValue);
        Assert.Equal(expectedValueEncoded, stringValue);
    }

    [Fact]
    public void ValueSet_OnBaseValueProperty_ShouldDeserializeToDescendantValue()
    {
        // Arrange
        var initialValue = new TestPayload("initial value");
        var cookie = Cookie.FromValue("test-cookie", initialValue);

        var newValue = new TestPayload("new value");
        var newValueSerialized = JsonSerializer.Serialize(newValue);
        var newValueEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(newValueSerialized));

        // Act
        (cookie as Cookie).Value = newValueEncoded;

        // Assert
        Assert.NotEqual(initialValue, newValue);

        Assert.NotEqual(initialValue, cookie.Value);
        Assert.Equal(newValue.Data, cookie.Value!.Data);
    }

    private class TestPayload(string data)
    {
        public string Data { get; set; } = data;
    }
}
