using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Semifinals.Framework.Services.Cosmos;

namespace IdentityService.Tests.E2E.Services.Cosmos;

[TestClass]
public class CosmosServiceTests
{
    public CosmosService Service = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        Service = new("DB CONNECTION STRING IS NEEDED HERE");
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
    [Ignore]
    public async Task CreateItemAsync_CreatesItem()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task UpsertItemAsync_CreatesItem()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task UpsertItemAsync_UpdatesItem()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task ReadItemAsync_ReadsExisting()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task ReadItemAsync_ReadsNonExisting()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task ReadManyItemsAsync_ReadsExisting()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task ReadManyItemsAsync_FailsReadingSomeNonExistent()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task ReadManyItemsAsync_FailsReadingAllNonExistent()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task PatchItemAsync_PatchesExisting()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task PatchItemAsync_FailsPatchingNonExistent()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task DeleteItemAsync_DeletesExisting()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task DeleteItemAsync_FailsDeletingNonExistent()
    {
    }

    [TestMethod]
    [Ignore]
    public async Task QueryAsync_HandlesQuery()
    {
    }
}