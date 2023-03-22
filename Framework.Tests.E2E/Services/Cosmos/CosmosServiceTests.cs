using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Semifinals.Framework.Services.Cosmos;
using Semifinals.Framework.Testing;

namespace Semifinals.Framework.Tests.E2E;

[TestClass]
public class CosmosServiceTests
{
    public CosmosService Service = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        Service = new(Environment.GetEnvironmentVariable("DbConnectionString")!);
    }

    [TestMethod]
    public async Task GetDatabaseAsync_CreatesNewOrExisting()
    {
        // Arrange
        string id = "test-db";

        // Act
        Database database = await Service.GetDatabaseAsync(id);

        // Assert
        Assert.IsNotNull(database);
    }

    [TestMethod]
    public async Task GetContainerAsync_FromDatabase_CreatesNewOrExisting()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        // Act
        Database database = await Service.GetDatabaseAsync(databaseId);
        Container container = await Service.GetContainerAsync(database, containerId, partitionKeyPath);

        // Assert
        Assert.IsNotNull(container);
    }

    [TestMethod]
    public async Task GetContainerAsync_FromIds_CreatesNewOrExisting()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        // Act
        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        // Assert
        Assert.IsNotNull(container);
    }

    [TestMethod]
    public async Task CreateItemAsync_CreatesItem()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item = new(Test.GenerateRandomString(), "Finley");

        // Act
        TestItem res = await Service.CreateItemAsync(container, item, item.Id);

        // Assert
        Assert.AreEqual(item.Id, res.Id);
    }

    [TestMethod]
    public async Task UpsertItemAsync_CreatesItem()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item = new(Test.GenerateRandomString(), "Finley");

        // Act
        TestItem res = await Service.UpsertItemAsync(container, item, item.Id);

        // Assert
        Assert.AreEqual(item.Id, res.Id);
    }

    [TestMethod]
    public async Task UpsertItemAsync_UpdatesItem()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item = new(Test.GenerateRandomString(), "Finley");
        await Service.CreateItemAsync(container, item, item.Id);

        item.Name = "Daniel";

        // Act
        TestItem res = await Service.UpsertItemAsync(container, item, item.Id);

        // Assert
        Assert.AreEqual(item.Id, res.Id);
        Assert.AreEqual("Daniel", res.Name);
    }

    [TestMethod]
    public async Task ReadItemAsync_ReadsExisting()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item = new(Test.GenerateRandomString(), "Finley");
        await Service.CreateItemAsync(container, item, item.Id);

        // Act
        TestItem res = await Service.ReadItemAsync<TestItem>(container, item.Id, item.Id);

        // Assert
        Assert.AreEqual(item.Id, res.Id);
        Assert.AreEqual(item.Name, res.Name);
    }

    [TestMethod]
    public async Task ReadItemAsync_ReadsNonExisting()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item = new(Test.GenerateRandomString(), "Finley");

        // Act
        async Task<TestItem> res() => await Service.ReadItemAsync<TestItem>(container, item.Id, item.Id);

        // Assert
        await Assert.ThrowsExceptionAsync<CosmosException>(res);
    }

    [TestMethod]
    public async Task ReadManyItemsAsync_ReadsExisting()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item1 = new(Test.GenerateRandomString(), "Finley");
        await Service.CreateItemAsync(container, item1, item1.Id);
        TestItem item2 = new(Test.GenerateRandomString(), "Daniel");
        await Service.CreateItemAsync(container, item2, item2.Id);

        List<(string, PartitionKey)> items = new()
        {
            (item1.Id, new(item1.Id)),
            (item2.Id, new(item2.Id))
        };

        // Act
        IEnumerable<TestItem> res = await Service.ReadManyItemsAsync<TestItem>(container, items);

        // Assert
        Assert.AreEqual(2, res.Count());
        Assert.IsTrue(res.Any(i => i.Id == item1.Id));
        Assert.IsTrue(res.Any(i => i.Id == item2.Id));
    }

    [TestMethod]
    public async Task ReadManyItemsAsync_FailsReadingSomeNonExistent()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item1 = new(Test.GenerateRandomString(), "Finley");
        await Service.CreateItemAsync(container, item1, item1.Id);

        List<(string, PartitionKey)> items = new()
        {
            (item1.Id, new(item1.Id)),
            ("DoesntExist", new("DoesntExist"))
        };

        // Act
        IEnumerable<TestItem> res = await Service.ReadManyItemsAsync<TestItem>(container, items);

        // Assert
        Assert.IsTrue(res.Count() == 1);
        Assert.AreEqual(item1.Id, res.First().Id);
    }

    [TestMethod]
    public async Task ReadManyItemsAsync_FailsReadingAllNonExistent()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        List<(string, PartitionKey)> items = new()
        {
            ("DoesntExist1", new("DoesntExist1")),
            ("DoesntExist2", new("DoesntExist2"))
        };

        // Act
        IEnumerable<TestItem> res = await Service.ReadManyItemsAsync<TestItem>(container, items);

        // Assert
        Assert.IsFalse(res.Any());
    }

    [TestMethod]
    public async Task PatchItemAsync_PatchesExisting()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item = new(Test.GenerateRandomString(), "Finley");
        await Service.CreateItemAsync(container, item, item.Id);

        List<PatchOperation> operations = new()
        {
            PatchOperation.Replace("/name", "Daniel")
        };

        // Act
        TestItem res = await Service.PatchItemAsync<TestItem>(container, item.Id, item.Id, operations);

        // Assert
        Assert.AreEqual("Daniel", res.Name);
    }

    [TestMethod]
    public async Task PatchItemAsync_FailsPatchingNonExistent()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        string id = Test.GenerateRandomString();

        List<PatchOperation> operations = new()
        {
            PatchOperation.Replace("/name", "Daniel")
        };

        // Act
        async Task<TestItem> res() => await Service.PatchItemAsync<TestItem>(container, id, id, operations);

        // Assert
        await Assert.ThrowsExceptionAsync<CosmosException>(res);
    }

    [TestMethod]
    public async Task DeleteItemAsync_DeletesExisting()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item = new(Test.GenerateRandomString(), "Finley");
        await Service.CreateItemAsync(container, item, new(item.Id));

        // Act
        TestItem? res = await Service.DeleteItemAsync<TestItem>(container, item.Id, item.Id);
        async Task<TestItem> get() => await Service.ReadItemAsync<TestItem>(container, item.Id, item.Id);

        // Assert
        Assert.IsNull(res);
        await Assert.ThrowsExceptionAsync<CosmosException>(get);
    }

    [TestMethod]
    public async Task DeleteItemAsync_FailsDeletingNonExistent()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item = new(Test.GenerateRandomString(), "Finley");

        // Act
        async Task<TestItem?> res() => await Service.DeleteItemAsync<TestItem>(container, item.Id, item.Id);

        // Assert
        await Assert.ThrowsExceptionAsync<CosmosException>(res);
    }

    [TestMethod]
    public async Task QueryAsync_HandlesQuery()
    {
        // Arrange
        string databaseId = "test-db";
        string containerId = "test";
        string partitionKeyPath = "/id";

        Container container = await Service.GetContainerAsync(databaseId, containerId, partitionKeyPath);

        TestItem item = new(Test.GenerateRandomString(), "Finley");
        await Service.CreateItemAsync(container, item, item.Id);

        // Act
        TestItem[] res = await Service.QueryAsync<TestItem>(container, query => query.Where(i => i.Id == item.Id));

        // Assert
        Assert.IsTrue(res.All(i => i.Id == item.Id));
    }
}

public class TestItem : CosmosItem
{
    [JsonProperty("name")]
    public string Name;

    public TestItem(string id, string name, DateTime? ts = null) : base(id, ts)
    {
        Name = name;
    }
}