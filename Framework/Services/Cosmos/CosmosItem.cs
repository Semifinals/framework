using Semifinals.Framework.Utils.Converters;
using Newtonsoft.Json;

namespace Semifinals.Framework.Services.Cosmos;

/// <summary>
/// Base class for an item stored in Cosmos Core-API DB.
/// </summary>
public abstract class CosmosItem
{
    public static readonly string PartitionKeyPath = "id";

    [JsonProperty("id")]
    public readonly string Id;

    [JsonProperty("_ts")]
    [JsonConverter(typeof(EpochConverter))]
    public readonly DateTime? Timestamp = null;

    public virtual bool ShouldSerializeTimestamp() => false;

    public CosmosItem(string id, DateTime? timestamp = null)
    {
        Id = id;
        Timestamp = timestamp;
    }
}