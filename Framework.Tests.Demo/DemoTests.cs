using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Semifinals.Framework.Demo;
using Semifinals.Framework.Testing;
using Semifinals.Framework.Utils;
using System;
using System.Threading.Tasks;

namespace Semifinals.Framework.Tests.Demo;

[TestClass]
public class DemoTests : Test
{
    [TestMethod]
    public async Task Ping_WorksWithoutBody()
    {
        // Arrange
        static async Task res() => await new RequestBuilder(HttpMethod.GET, "http://localhost:7092/api/ping")
            .Handle(200, async body =>
            {
                await Task.Delay(1);
            })
            .Call();

        // Act
        await res();

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task Ping_WorksWithBody()
    {
        // Arrange
        static async Task res() => await new RequestBuilder(HttpMethod.GET, "http://localhost:7092/api/ping")
            .AddBody<TestDto>(new() { valid = true })
            .Handle(200, async body =>
            {
                await Task.Delay(1);
            })
            .Call();

        // Act
        await res();

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task Body_WorksWithBody()
    {
        // Arrange
        static async Task res() => await new RequestBuilder(HttpMethod.POST, "http://localhost:7092/api/body")
            .AddBody<TestDto>(new() { valid = true })
            .Handle(200, async body =>
            {
                await Task.Delay(1);
            })
            .Call();

        // Act
        await res();

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task Body_FailsWithoutBody()
    {
        // Arrange
        static async Task res() => await new RequestBuilder(HttpMethod.POST, "http://localhost:7092/api/body")
            .Handle(400, async body =>
            {
                await Task.Delay(1);
            })
            .Call();

        // Act
        static async Task call() => await res();
        await call();

        // Assert
        Assert.IsTrue(true);
    }
}