using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Semifinals.Framework.Services.Cosmos;

/// <summary>
/// Service handling communication with an Azure Cosmos Core-API DB.
/// </summary>
public class CosmosService : IDisposable
{
    private readonly string DbConnectionString;
    private readonly CosmosClient Client;

    public double RequestCharge { get; private set; } = 0;

    public CosmosService(string dbConnectionString)
    {
        DbConnectionString = dbConnectionString;
        Client = new(DbConnectionString);
    }

    public void Dispose()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Access a database.
    /// </summary>
    /// <param name="id">The ID for the database to access</param>
    /// <returns>The database object</returns>
    public async Task<Database> GetDatabaseAsync(string id)
    {
        DatabaseResponse response = await Client.CreateDatabaseIfNotExistsAsync(id);
        RequestCharge += response.RequestCharge;
        return response.Database;
    }

    /// <summary>
    /// Access a container inside a given database.
    /// </summary>
    /// <param name="database">The database the container is within</param>
    /// <param name="id">The ID for the container</param>
    /// <param name="partitionKeyPath">The partition key path for the container</param>
    /// <returns>The container object</returns>
    public async Task<Container> GetContainerAsync(
        Database database,
        string id,
        string partitionKeyPath)
    {
        ContainerResponse response = await database.CreateContainerIfNotExistsAsync(id, partitionKeyPath);
        RequestCharge += response.RequestCharge;
        return response.Container;
    }

    /// <summary>
    /// Access a container inside a given database using string identifiers for both.
    /// </summary>
    /// <param name="databaseId">The ID for the database</param>
    /// <param name="containerId">The ID for the container</param>
    /// <param name="partitionKeyPath">The partition key path for the container</param>
    /// <returns>The container object</returns>
    public async Task<Container> GetContainerAsync(
        string databaseId,
        string containerId,
        string partitionKeyPath)
    {
        Database database = await GetDatabaseAsync(databaseId);
        Container container = await GetContainerAsync(database, containerId, partitionKeyPath);
        return container;
    }

    /// <summary>
    /// Create a new item in the container.
    /// </summary>
    /// <typeparam name="T">The type of the cosmos item to be created</typeparam>
    /// <param name="container">The container to create the item in</param>
    /// <param name="item">The item to be added to the database</param>
    /// <param name="partitionKey">The partition key for the item</param>
    /// <returns>The created item from the database</returns>
    public async Task<T> CreateItemAsync<T>(
        Container container,
        T item,
        string partitionKey)
        where T : CosmosItem
    {
        ItemResponse<T> response = await container.CreateItemAsync(item, new(partitionKey));
        RequestCharge += response.RequestCharge;
        return response.Resource;
    }

    /// <summary>
    /// Create or replace an existing item in the container.
    /// </summary>
    /// <typeparam name="T">The type of the cosmos item to be put</typeparam>
    /// <param name="container">The container to put the item in</param>
    /// <param name="item">The item to be put in the database</param>
    /// <param name="partitionKey">The partition key for the item</param>
    /// <returns>The resulting item from the database</returns>
    public async Task<T> UpsertItemAsync<T>(
        Container container,
        T item,
        string partitionKey)
        where T : CosmosItem
    {
        ItemResponse<T> response = await container.UpsertItemAsync(item, new(partitionKey));
        RequestCharge += response.RequestCharge;
        return response.Resource;
    }

    /// <summary>
    /// Get an item from the database.
    /// </summary>
    /// <typeparam name="T">The type of the cosmos item to get</typeparam>
    /// <param name="container">The container to get the item from</param>
    /// <param name="itemId">The ID for the item to get</param>
    /// <param name="partitionKey">The partition key for the item to get</param>
    /// <returns>The item that was requested</returns>
    public async Task<T> ReadItemAsync<T>(
        Container container,
        string itemId,
        string partitionKey)
        where T : CosmosItem
    {
        ItemResponse<T> response = await container.ReadItemAsync<T>(itemId, new(partitionKey));
        RequestCharge += response.RequestCharge;
        return response.Resource;
    }

    /// <summary>
    /// Get many items from the database.
    /// </summary>
    /// <typeparam name="T">The type of the cosmos items to get</typeparam>
    /// <param name="container">The container to get the item from</param>
    /// <param name="items">A list of tuples with the ID and partition keys of the items to get</param>
    /// <returns>The items that were requested</returns>
    public async Task<IEnumerable<T>> ReadManyItemsAsync<T>(
        Container container,
        IReadOnlyList<(string, PartitionKey)> items)
        where T : CosmosItem
    {
        FeedResponse<T> response = await container.ReadManyItemsAsync<T>(items);
        RequestCharge += response.RequestCharge;
        return response.Resource;
    }

    /// <summary>
    /// Modify an item in the database.
    /// </summary>
    /// <typeparam name="T">The type of the cosmos items to modify</typeparam>
    /// <param name="container">The container to find the item in</param>
    /// <param name="id">The ID for the item to modify</param>
    /// <param name="partitionKey">The partition key for the item to modify</param>
    /// <param name="operations">A list of operations to make to modify the item</param>
    /// <returns>The item after changes are made</returns>
    public async Task<T> PatchItemAsync<T>(
        Container container,
        string id,
        PartitionKey partitionKey,
        IReadOnlyList<PatchOperation> operations)
        where T : CosmosItem
    {
        ItemResponse<T> response = await container.PatchItemAsync<T>(id, partitionKey, operations);
        RequestCharge += response.RequestCharge;
        return response.Resource;
    }

    /// <summary>
    /// Delete an item from the database.
    /// </summary>
    /// <typeparam name="T">The type of the cosmos items to delete</typeparam>
    /// <param name="container">The container to find the item in</param>
    /// <param name="id">The ID for the item to delete</param>
    /// <param name="partitionKey">The partition key for the item to delete</param>
    /// <returns>The deleted item</returns>
    public async Task<T> DeleteItemAsync<T>(
        Container container,
        string id,
        PartitionKey partitionKey)
        where T : CosmosItem
    {
        ItemResponse<T> response = await container.DeleteItemAsync<T>(id, partitionKey);
        RequestCharge += response.RequestCharge;
        return response.Resource;
    }
    
    /// <summary>
    /// Submit a query to the database.
    /// </summary>
    /// <typeparam name="T">The type of the cosmos items to return</typeparam>
    /// <param name="container">The container to query</param>
    /// <param name="query">A function used to add to the linq query</param>
    /// <returns>The result of the query made</returns>
    public async Task<T[]> QueryAsync<T>(
        Container container,
        Func<IQueryable<T>, IQueryable<T>> query)
        where T : CosmosItem
    {
        IQueryable<T> queryable = container.GetItemLinqQueryable<T>();

        using FeedIterator<T> linqFeed = query(queryable).ToFeedIterator();

        List<T> items = new();

        while (linqFeed.HasMoreResults)
        {
            FeedResponse<T> response = await linqFeed.ReadNextAsync();
            RequestCharge += response.RequestCharge;

            foreach (T item in response)
            {
                items.Add(item);
            }
        }

        return items.ToArray();
    }
}