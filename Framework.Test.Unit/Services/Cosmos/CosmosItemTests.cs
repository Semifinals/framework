using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semifinals.Framework.Services.Cosmos;

namespace IdentityService.Tests.Unit.Services.Cosmos;

[TestClass]
public class CosmosItemTests
{
    [TestMethod]
    public void JsonConvert_ShouldDeserializeWithTimestamp()
    {
        // Arrange
        string id = "ThisIsAValidId12";
        string itemJson = @$"
            {{
                ""id"": ""{id}"",
                ""_ts"": 0
            }}
        ";

        // Act
        UnabstractCosmosItem item = JsonConvert.DeserializeObject<UnabstractCosmosItem>(itemJson);

        // Assert
        Assert.IsNotNull(item);
        Assert.IsNotNull(item.Timestamp);
        Assert.AreEqual(id, item.Id);
        Assert.AreEqual(DateTime.UnixEpoch, item.Timestamp);
    }

    [TestMethod]
    public void JsonConvert_ShouldDeserializeWithoutTimestamp()
    {
        // Arrange
        string id = "ThisIsAValidId12";
        string itemJson = @$"
            {{
                ""id"": ""{id}""
            }}
        ";

        // Act
        UnabstractCosmosItem item = JsonConvert.DeserializeObject<UnabstractCosmosItem>(itemJson);

        // Assert
        Assert.IsNotNull(item);
        Assert.IsNull(item.Timestamp);
        Assert.AreEqual(id, item.Id);
    }

    [TestMethod]
    public void JsonConvert_ShouldSerializeWithTimestamp()
    {
        // Arrange
        string id = "ThisIsAValidId12";
        DateTime timestamp = DateTime.UnixEpoch;
        UnabstractCosmosItemTsOverride item = new(id, timestamp);

        // Act
        string itemJson = JsonConvert.SerializeObject(item);
        JObject jsonObject = JObject.Parse(itemJson);

        // Assert
        Assert.IsTrue(jsonObject.ContainsKey("_ts"));
        Assert.AreEqual(0, jsonObject.Property("_ts").Value);
        Assert.AreEqual(id, jsonObject.Property("id").Value);
    }

    [TestMethod]
    public void JsonConvert_ShouldSerializeWithoutTimestamp()
    {
        // Arrange
        string id = "ThisIsAValidId12";
        DateTime timestamp = DateTime.UnixEpoch;
        UnabstractCosmosItem item = new(id, timestamp);

        // Act
        string itemJson = JsonConvert.SerializeObject(item);
        JObject jsonObject = JObject.Parse(itemJson);

        // Assert
        Assert.IsFalse(jsonObject.ContainsKey("_ts"));
        Assert.AreEqual(id, jsonObject.Property("id").Value);
    }
}

public class UnabstractCosmosItem : CosmosItem
{
    public UnabstractCosmosItem(string id, DateTime? timestamp = null) : base(id, timestamp) { }
}

public class UnabstractCosmosItemTsOverride : CosmosItem
{
    public UnabstractCosmosItemTsOverride(string id, DateTime? timestamp = null) : base(id, timestamp) { }

    public override bool ShouldSerializeTimestamp() => true;
}