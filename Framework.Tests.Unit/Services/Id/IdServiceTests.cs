using Microsoft.VisualStudio.TestTools.UnitTesting;
using Semifinals.Framework.Services.Id;

namespace Semifinals.Framework.Tests.Unit;

[TestClass]
public class IdServiceTests
{
    [TestMethod]
    public void Generate_ShouldGenerateId()
    {
        // Arrange

        // Act
        string id = IdService.Generate();

        // Assert
        Assert.IsNotNull(id);
        Assert.AreEqual(16, id.Length);
    }
}