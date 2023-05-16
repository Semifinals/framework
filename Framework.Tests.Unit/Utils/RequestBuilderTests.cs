using Microsoft.VisualStudio.TestTools.UnitTesting;
using Semifinals.Framework;
using Semifinals.Framework.Utils;

namespace Semifinals.Framework.Tests.Unit;

[TestClass]
public class RequestBuilderTests
{
    [TestMethod]
    public void Constructor_FormatsRequest()
    {
        // Arrange
        HttpMethod method = HttpMethod.GET;
        string uri = "";

        // Act
        RequestBuilder builder = new(method, uri);

        // Assert
        Assert.AreEqual(method, builder.Method);
        Assert.AreEqual(uri, builder.Url);
        Assert.AreEqual(0, builder.Handlers.Count);
    }

    [TestMethod]
    public void Handle_AddsNewHandler()
    {
        // Arrange
        HttpMethod method = HttpMethod.GET;
        string uri = "";
        RequestBuilder builder = new(method, uri);

        // Act
        builder.Handle(200, async body =>
        {
            await Task.Delay(1);
        });

        // Assert
        Assert.AreEqual(1, builder.Handlers.Count);
        Assert.IsNotNull(builder.Handlers[200]);
    }

    [TestMethod]
    public void AddBody_AddsNewBody()
    {
        // Arrange
        HttpMethod method = HttpMethod.GET;
        string uri = "";
        RequestBuilder builder = new(method, uri);

        // Act
        builder.AddBody<NoBodyDto>(new());

        // Assert
        Assert.IsInstanceOfType(builder.Body, typeof(NoBodyDto));
    }

    [TestMethod]
    public void AddBody_OverwritesExistingBody()
    {
        // Arrange
        HttpMethod method = HttpMethod.GET;
        string uri = "";
        RequestBuilder builder = new(method, uri);

        // Act
        builder.AddBody<NoBodyDto>(new());
        builder.AddBody<TestBodyDto>(new());

        // Assert
        Assert.IsInstanceOfType(builder.Body, typeof(TestBodyDto));
    }

    [TestMethod]
    public void AddHeader_AddsHeader()
    {
        // Arrange
        HttpMethod method = HttpMethod.GET;
        string uri = "";
        RequestBuilder builder = new(method, uri);

        // Act
        builder.AddHeader("example", "value");

        // Assert
        Assert.AreEqual(1, builder.Headers.Count);
        Assert.IsTrue(builder.Headers.ContainsKey("example"));
    }

    [TestMethod]
    public void AddBearerAuthorizationHeaders_AddsHeader()
    {
        // Arrange
        HttpMethod method = HttpMethod.GET;
        string uri = "";
        RequestBuilder builder = new(method, uri);

        // Act
        builder.AddBearerAuthorizationHeader("token");

        // Assert
        Assert.AreEqual(1, builder.Headers.Count);
        Assert.IsTrue(builder.Headers.ContainsKey("Authorization"));
        Assert.AreEqual("Bearer token", builder.Headers["Authorization"]);
    }

    [TestMethod]
    public async Task Call_HandlesValidRequest()
    {
        // Arrange
        HttpMethod method = HttpMethod.GET;
        string uri = "https://example.com";
        RequestBuilder builder = new(method, uri);

        bool success = false;

        // Act
        builder.Handle(200, async body =>
        {
            await Task.Delay(1);
            success = true;
        });

        await builder.Call();

        // Assert
        Assert.IsTrue(success);
    }

    [TestMethod]
    public async Task Call_FailsWithMissingHandler()
    {
        // Arrange
        HttpMethod method = HttpMethod.GET;
        string uri = "https://example.com";
        RequestBuilder builder = new(method, uri);

        // Act
        async Task res() => await builder.Call();

        // Assert
        await Assert.ThrowsExceptionAsync<NotHandledException>(res);
    }

    [TestMethod]
    public async Task Call_FailsOnInvalidEndpoint()
    {
        // Arrange
        HttpMethod method = HttpMethod.GET;
        string uri = "";
        RequestBuilder builder = new(method, uri);

        // Act
        async Task res() => await builder.Call();

        // Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(res);
    }
}