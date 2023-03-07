using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Semifinals.Framework.Utils.Converters;

public class EpochConverter : DateTimeConverterBase
{
    private static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteRawValue(Math.Round(((DateTime)value - Epoch).TotalMilliseconds).ToString());
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;

        return Epoch.AddMilliseconds((long)reader.Value);
    }
}