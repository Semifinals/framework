using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semifinals.Framework.Utils.Converters;

namespace Semifinals.Framework.Tests.Unit;

[TestClass]
public class EpochConverterTests
{
    [TestMethod]
    public void WriteJson_SerializeShouldWork()
    {
        // Arrange
        DateTime value = new(2023, 2, 5, 0, 0, 0, DateTimeKind.Utc);
        TimeObject timeObject = new(value);

        // Act
        string serialized = JsonConvert.SerializeObject(timeObject);
        JObject jsonObject = JObject.Parse(serialized);

        // Assert
        Assert.IsNotNull(jsonObject);
        Assert.IsTrue(jsonObject.ContainsKey("time"));
        Assert.AreEqual(new DateTimeOffset(value).ToUnixTimeMilliseconds(), jsonObject.Property("time")!.Value);
    }

    [TestMethod]
    [DataRow()]
    public void ReadJson_DeserializeShouldWork()
    {
        // Arrange
        long value = 1675570591000;
        string json = $@"{{ ""time"": {value} }}";

        // Act
        TimeObject deserialized = JsonConvert.DeserializeObject<TimeObject>(json)!;

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(DateTimeOffset.FromUnixTimeMilliseconds(value).DateTime, deserialized.Time);
    }
}

public class TimeObject
{
    [JsonProperty("time")]
    [JsonConverter(typeof(EpochConverter))]
    public readonly DateTime Time;

    public TimeObject(DateTime time)
    {
        Time = time;
    }
}