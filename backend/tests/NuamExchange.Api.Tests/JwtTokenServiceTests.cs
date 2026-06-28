using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuamExchange.Application.Authentication;
using NuamExchange.Infrastructure.Authentication;
using Xunit;

namespace NuamExchange.Api.Tests;

public sealed class JwtTokenServiceTests
{
    private const string ValidIssuer = "NuamExchange.Tests";
    private const string ValidAudience = "NuamExchange.Tests.Client";
    private const string ValidSigningKey = "test-signing-key-with-at-least-32-characters";
    private const int ValidAccessTokenMinutes = 45;

    private static readonly DateTimeOffset FixedNow = new(2026, 6, 28, 12, 30, 0, TimeSpan.Zero);

    [Fact]
    public void GenerateTokenWithValidConfigurationReturnsSignedValidToken()
    {
        var options = CreateOptions();
        var service = CreateService(options);
        var request = new JwtTokenRequest(
            UserId: "user-123",
            Email: "user@example.test",
            FullName: "Usuario Prueba",
            Roles: ["Administrador", "Supervisor", "Administrador", " "]);

        var result = service.GenerateToken(request);

        Assert.Equal(FixedNow.AddMinutes(options.AccessTokenMinutes), result.ExpiresAtUtc);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
        Assert.Equal(SecurityAlgorithms.HmacSha256, jwt.Header.Alg);

        var principal = ValidateToken(result.AccessToken, options);
        Assert.Equal("user-123", principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value);
        Assert.Equal("user@example.test", principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value);
        Assert.Equal("Usuario Prueba", principal.FindFirst(JwtRegisteredClaimNames.Name)?.Value);
        Assert.Equal(
            FixedNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
            principal.FindFirst(JwtRegisteredClaimNames.Iat)?.Value);
        Assert.Single(principal.FindAll(JwtRegisteredClaimNames.Sub));
        Assert.Single(principal.FindAll(JwtRegisteredClaimNames.Email));
        Assert.Single(principal.FindAll(JwtRegisteredClaimNames.Name));
        Assert.Single(principal.FindAll(ClaimTypes.Role), claim => claim.Value == "Administrador");
        Assert.Single(principal.FindAll(ClaimTypes.Role), claim => claim.Value == "Supervisor");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GenerateTokenRejectsEmptySigningKey(string? signingKey)
    {
        var options = CreateOptions(signingKey: signingKey);
        var service = CreateService(options);

        var exception = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(CreateRequest()));
        Assert.Contains("signing key", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateTokenRejectsSigningKeyShorterThanThirtyTwoCharacters()
    {
        var options = CreateOptions(signingKey: "short-signing-key");
        var service = CreateService(options);

        var exception = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(CreateRequest()));
        Assert.Contains("at least 32", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null, ValidAudience)]
    [InlineData("", ValidAudience)]
    [InlineData("   ", ValidAudience)]
    [InlineData(ValidIssuer, null)]
    [InlineData(ValidIssuer, "")]
    [InlineData(ValidIssuer, "   ")]
    public void GenerateTokenRejectsEmptyIssuerOrAudience(string? issuer, string? audience)
    {
        var options = CreateOptions(issuer: issuer, audience: audience);
        var service = CreateService(options);

        var exception = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(CreateRequest()));
        Assert.True(
            exception.Message.Contains("issuer", StringComparison.OrdinalIgnoreCase)
            || exception.Message.Contains("audience", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GenerateTokenRejectsInvalidExpiration(int accessTokenMinutes)
    {
        var options = CreateOptions(accessTokenMinutes: accessTokenMinutes);
        var service = CreateService(options);

        var exception = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(CreateRequest()));
        Assert.Contains("expiration", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateTokenRejectsNullRequest()
    {
        var service = CreateService(CreateOptions());

        Assert.Throws<ArgumentNullException>(() => service.GenerateToken(null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GenerateTokenRejectsEmptyUserId(string? userId)
    {
        var request = CreateRequest() with { UserId = userId! };
        var service = CreateService(CreateOptions());

        Assert.Throws<ArgumentException>(() => service.GenerateToken(request));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GenerateTokenRejectsEmptyEmail(string? email)
    {
        var request = CreateRequest() with { Email = email! };
        var service = CreateService(CreateOptions());

        Assert.Throws<ArgumentException>(() => service.GenerateToken(request));
    }

    [Fact]
    public void GenerateTokenRejectsNullRoles()
    {
        var request = CreateRequest() with { Roles = null! };
        var service = CreateService(CreateOptions());

        Assert.Throws<ArgumentNullException>(() => service.GenerateToken(request));
    }

    private static JwtTokenService CreateService(JwtOptions options) =>
        new(Options.Create(options), new FixedTimeProvider(FixedNow));

    private static JwtOptions CreateOptions(
        string? issuer = ValidIssuer,
        string? audience = ValidAudience,
        string? signingKey = ValidSigningKey,
        int accessTokenMinutes = ValidAccessTokenMinutes) =>
        new()
        {
            Issuer = issuer,
            Audience = audience,
            SigningKey = signingKey,
            AccessTokenMinutes = accessTokenMinutes,
        };

    private static JwtTokenRequest CreateRequest() =>
        new("user-123", "user@example.test", "Usuario Prueba", ["Administrador"]);

    private static ClaimsPrincipal ValidateToken(string accessToken, JwtOptions options)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            LifetimeValidator = (notBefore, expires, _, _) =>
                notBefore == FixedNow.UtcDateTime
                && expires == FixedNow.AddMinutes(options.AccessTokenMinutes).UtcDateTime,
        };

        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        return handler.ValidateToken(accessToken, parameters, out _);
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
