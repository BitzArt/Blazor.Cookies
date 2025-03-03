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

        var expectedStringValue = JsonSerializer.Serialize(value);
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

        var initialStringValue = JsonSerializer.Serialize(initialValue);
        var expectedStringValue = JsonSerializer.Serialize(newValue);

        var stringValue = (cookie as Cookie).Value;

        Assert.NotEqual(initialStringValue, stringValue);
        Assert.Equal(expectedStringValue, stringValue);
    }

    [Fact]
    public void ValueSet_OnBaseValueProperty_ShouldDeserializeToDescendantValue()
    {
        // Arrange
        var initialValue = new TestPayload("initial value");
        var cookie = Cookie.FromValue("test-cookie", initialValue);

        var newValue = new TestPayload("new value");
        var newValueSerialized = JsonSerializer.Serialize(newValue);

        // Act
        (cookie as Cookie).Value = newValueSerialized;

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