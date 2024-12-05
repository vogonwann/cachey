using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cachey.Common;

public class CacheItemConverter : JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(CacheItem<>);
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var genericType = typeToConvert.GetGenericArguments()[0];
        using (var doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;

            var value = JsonSerializer.Deserialize(root.GetProperty("Value").GetRawText(), genericType, options);
            var expirationTime = root.TryGetProperty("ExpirationTime", out var expirationProperty)
                ? expirationProperty.GetDateTime()
                : (DateTime?)null;

            var cacheItemType = typeof(CacheItem<>).MakeGenericType(genericType);
            var cacheItemCtor = cacheItemType.GetConstructor(new[] { genericType, typeof(TimeSpan?) });
            return cacheItemCtor.Invoke(new[]
                { value, expirationTime.HasValue ? (TimeSpan?)expirationTime.Value.Subtract(DateTime.UtcNow) : null });
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var cacheItemType = value.GetType();
        var genericType = cacheItemType.GetGenericArguments()[0];
        var cacheItem = (CacheItem<object>)value;

        writer.WriteStartObject();
        writer.WriteEndObject();
    }
}