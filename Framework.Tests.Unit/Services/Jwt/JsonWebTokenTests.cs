using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Semifinals.Framework.Services.Jwt;
using System.Text;

namespace Semifinals.Framework.Tests.Unit;

[TestClass]
public class JsonWebTokenTests
{

    [TestMethod]
    public void Constructor_ShouldGenerateFromContent()
    {
        // Arrange
        JwtTestPayload content = new();
        string subject = "subject";

        // Act
        JsonWebToken<JwtTestPayload> jwt = new(content, subject);

        // Assert
        Assert.IsNotNull(jwt);
        Assert.AreEqual(subject, jwt.Payload.Subject);
        Assert.AreEqual(content, jwt.Payload.Content);
    }

    [TestMethod]
    public void Constructor_ShouldGenerateFromPayload()
    {
        // Arrange
        JwtTestPayload content = new();
        string subject = "subject";

        // Act
        JsonWebToken<JwtTestPayload> jwt = new(content, subject);

        // Assert
        Assert.IsNotNull(jwt);
        Assert.AreEqual(subject, jwt.Payload.Subject);
        Assert.AreEqual(content, jwt.Payload.Content);
    }
}

[TestClass]
public class JwtHeaderTests
{
    [TestMethod]
    public void JsonConvert_ShouldDeserialize()
    {
        // Arrange
        string validHeader = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";

        // Act
        byte[] headerBytes = Convert.FromBase64String(validHeader);
        string headerJson = Encoding.UTF8.GetString(headerBytes);
        JwtHeader header = JsonConvert.DeserializeObject<JwtHeader>(headerJson)!;

        // Assert
        Assert.IsNotNull(header);
    }

    [TestMethod]
    public void ToString_ShouldEncode()
    {
        // Arrange
        JwtHeader validHeader = new();

        // Act
        string header = validHeader.ToString();

        // Assert
        Assert.AreEqual("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", header);
    }
}

[TestClass]
public class JwtPayloadTests
{
    [TestMethod]
    public void JsonConvert_ShouldDeserializeWithoutContent()
    {
        // Arrange
        string validPayload = "eyJpYXQiOjE2NzU1NzMzOTA2OTksImV4cCI6MTY3NjE3ODE5MDY5OSwic3ViIjoic3ViamVjdCIsIm5vbmNlIjoic29tZW5vbmNlIn0=";

        // Act
        byte[] payloadBytes = Convert.FromBase64String(validPayload);
        string payloadJson = Encoding.UTF8.GetString(payloadBytes);
        JwtPayload<JwtTestPayload> payload = JsonConvert.DeserializeObject<JwtPayload<JwtTestPayload>>(payloadJson)!;

        // Assert
        Assert.IsNotNull(payload);
        Assert.AreEqual("subject", payload.Subject);
        Assert.IsNull(payload.Content);
    }

    [TestMethod]
    public void ToString_ShouldEncode()
    {
        // Arrange
        JwtPayload<JwtTestPayload> payload = new(new(), "subject");

        // Act
        string payloadEncoded = payload.ToString();

        // Assert
        Assert.IsNotNull(payloadEncoded);
    }
}

public class JwtTestPayload
{
    public static readonly string DefaultNonce = "somenonce";

    [JsonProperty("nonce")]
    public readonly string Nonce;

    public JwtTestPayload()
    {
        Nonce = DefaultNonce;
    }
}