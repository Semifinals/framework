using Microsoft.VisualStudio.TestTools.UnitTesting;
using Semifinals.Framework.Services.Jwt;

namespace Semifinals.Framework.Tests.Unit;

[TestClass]
public class JwtServiceTests
{
    [TestMethod]
    public void Validate_ShouldWorkOnValid()
    {
        // Arrange
        JsonWebToken<JwtTestPayload> jwtInstance = new(new(), "subject");
        string jwt = JwtService.Generate(jwtInstance, "jwtSecret");

        // Act
        bool valid = JwtService.Validate(jwt);

        // Assert
        Assert.IsTrue(valid);
    }

    [TestMethod]
    public void Validate_ShouldFailOnInvalidFormat()
    {
        // Arrange
        string invalidJwt = "ThisDoesNotHaveEnough.Dots";

        // Act
        bool valid = JwtService.Validate(invalidJwt);

        // Assert
        Assert.IsFalse(valid);
    }

    [TestMethod]
    public void Validate_ShouldFailOnInvalidHeader()
    {
        // Arrange
        string invalidJwt = "IncorrectHeader.InThis.JsonWebToken";

        // Act
        bool valid = JwtService.Validate(invalidJwt);

        // Assert
        Assert.IsFalse(valid);
    }

    [TestMethod]
    public void Verify_ShouldWorkOnValid()
    {
        // Arrange
        JsonWebToken<JwtTestPayload> jwtInstance = new(new(), "subject");
        string jwt = JwtService.Generate(jwtInstance, "jwtSecret");

        // Act
        bool verified = JwtService.Verify(jwt, "jwtSecret");

        // Assert
        Assert.IsTrue(verified);
    }

    [TestMethod]
    public void Verify_ShouldFailOnWrongPayload()
    {
        // Arrange
        JsonWebToken<JwtTestPayload> jwtInstance = new(new(), "subject");
        string validJwt = JwtService.Generate(jwtInstance, "jwtSecret");
        string[] parts = validJwt.Split('.');

        string invalidJwt = $"{parts[0]}.MaliciousPayload.{parts[2]}";

        // Act
        bool verified = JwtService.Verify(invalidJwt, "jwtSecret");

        // Assert
        Assert.IsFalse(verified);
    }

    [TestMethod]
    public void Verify_ShouldFailOnWrongSignature()
    {
        // Arrange
        JsonWebToken<JwtTestPayload> jwtInstance = new(new(), "subject");
        string validJwt = JwtService.Generate(jwtInstance, "jwtSecret");
        string[] parts = validJwt.Split('.');

        string invalidJwt = $"{parts[0]}.{parts[1]}.MaliciousSignature";

        // Act
        bool verified = JwtService.Verify(invalidJwt, "jwtSecret");

        // Assert
        Assert.IsFalse(verified);
    }

    [TestMethod]
    public void TryParse_ShouldParseGoodJwt()
    {
        // Arrange
        string subject = "subject";

        JsonWebToken<JwtTestPayload> jwtInstance = new(new(), subject);
        string jwt = JwtService.Generate(jwtInstance, "jwtSecret");

        // Act
        bool parsed = JwtService.TryParse(jwt, "jwtSecret", out JsonWebToken<JwtTestPayload>? jwtParsed);

        // Assert
        Assert.IsTrue(parsed);
        Assert.IsNotNull(jwtParsed);
        Assert.AreEqual(subject, jwtParsed.Payload.Subject);
        Assert.AreEqual(JwtTestPayload.DefaultNonce, jwtParsed.Payload.Content.Nonce);
    }

    [TestMethod]
    public void TryParse_ShouldNotParseInvalid()
    {
        // Arrange
        string invalidJwt = "ThisDoesNotHaveEnough.Dots";

        // Act
        bool parsed = JwtService.TryParse(invalidJwt, "jwtSecret", out JsonWebToken<JwtTestPayload>? jwtParsed);

        // Assert
        Assert.IsFalse(parsed);
        Assert.IsNull(jwtParsed);
    }

    [TestMethod]
    public void TryParse_ShouldNotParseNotVerified()
    {
        // Arrange
        JsonWebToken<JwtTestPayload> jwtInstance = new(new(), "subject");
        string validJwt = JwtService.Generate(jwtInstance, "jwtSecret");
        string[] parts = validJwt.Split('.');

        string invalidJwt = $"{parts[0]}.MaliciousPayload.{parts[2]}";

        // Act
        bool parsed = JwtService.TryParse(invalidJwt, "jwtSecret", out JsonWebToken<JwtTestPayload>? jwtParsed);

        // Assert
        Assert.IsFalse(parsed);
        Assert.IsNull(jwtParsed);
    }

    [TestMethod]
    public void GenerateSignature_FromJwt_ShouldGenerateCorrectSignature()
    {
        // Arrange
        string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE2NzgxODgyNDkxMTYsImV4cCI6MTY3ODc5MzA0OTExNiwic3ViIjoic3ViamVjdCIsIm5vbmNlIjoic29tZW5vbmNlIn0=.gBCAhc4DGq9mi8oEJWKVWS/u7CXg6z/fTJs2+WhMf7s=";
        if (JwtService.TryParse(jwt, "jwtSecret", out JsonWebToken<JwtTestPayload>? jwtParsed)) { }

        string expectedSignature = jwt.Split('.')[2];

        // Act
        string signature = JwtService.GenerateSignature(jwtParsed!, "jwtSecret");

        // Assert
        Assert.AreEqual(expectedSignature, signature);
    }

    [TestMethod]
    public void GenerateSignature_FromContent_ShouldGenerateCorrectSignature()
    {
        // Arrange
        string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE2NzgxODgyNDkxMTYsImV4cCI6MTY3ODc5MzA0OTExNiwic3ViIjoic3ViamVjdCIsIm5vbmNlIjoic29tZW5vbmNlIn0=.gBCAhc4DGq9mi8oEJWKVWS/u7CXg6z/fTJs2+WhMf7s=";

        string[] parts = jwt.Split('.');
        string header = parts[0];
        string payload = parts[1];
        string expectedSignature = parts[2];

        // Act
        string signature = JwtService.GenerateSignature(header, payload, "jwtSecret");

        // Assert
        Assert.AreEqual(expectedSignature, signature);
    }

    [TestMethod]
    public void Generate_GeneratesJWT()
    {
        // Arrange
        string startingJwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE2NzgxODgyNDkxMTYsImV4cCI6MTY3ODc5MzA0OTExNiwic3ViIjoic3ViamVjdCIsIm5vbmNlIjoic29tZW5vbmNlIn0=.gBCAhc4DGq9mi8oEJWKVWS/u7CXg6z/fTJs2+WhMf7s=";
        if (JwtService.TryParse(startingJwt, "jwtSecret", out JsonWebToken<JwtTestPayload>? jwtParsed)) { }

        // Act
        string jwt = JwtService.Generate(jwtParsed!, "jwtSecret");

        // Assert
        Assert.AreEqual("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE2NzgxODgyNDkxMTYsImV4cCI6MTY3ODc5MzA0OTExNiwic3ViIjoic3ViamVjdCIsIm5vbmNlIjoic29tZW5vbmNlIn0=.gBCAhc4DGq9mi8oEJWKVWS/u7CXg6z/fTJs2+WhMf7s=", jwt);
    }
}