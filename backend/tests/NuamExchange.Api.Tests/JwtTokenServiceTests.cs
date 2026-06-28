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
    private static readonly DateTimeOffset FixedNow = new(2026, 6, 28, 12, 30, 0, TimeSpan.Zero);

    [Fact]
    public void GenerateTokenWithValidConfigurationReturnsSignedValidToken()
    {
        var options = CreateValidOptions();
        var service = CreateService(options);
        var request = new JwtTokenRequest(
            UserId: "user-123",
            Email: "user@example.test",
            FullName: "Usuario Prueba",
            Roles: ["Administrador", "Supervisor"]);

        var result = service.GenerateToken(request);

        Assert.Equal(FixedNow.AddMinutes(options.AccessTokenMinutes), result.ExpiresAtUtc);
        var principal = ValidateToken(result.AccessToken, options);
        Assert.Equal("user-123", principal.FindFirstValue(JwtRegisteredClaimNames.Sub));
        Assert.Equal("user-123", principal.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Equal("user@example.test", principal.FindFirstValue(JwtRegisteredClaimNames.Email));
        Assert.Equal("Usuario Prueba", principal.FindFirstValue(JwtRegisteredClaimNames.Name));
        Assert.Contains(principal.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Administrador");
        Assert.Contains(principal.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Supervisor");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GenerateTokenRejectsEmptySigningKey(string? signingKey)
    {
        var options = CreateValidOptions() with { SigningKey = signingKey };
        var service = CreateService(options);

        var exception = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(CreateRequest()));
        Assert.Contains("signing key", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateTokenRejectsSigningKeyShorterThanThirtyTwoCharacters()
    {
        var options = CreateValidOptions() with { SigningKey = "short-signing-key" };
        var service = CreateService(options);

        var exception = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(CreateRequest()));
        Assert.Contains("at least 32", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null, "audience")]
    [InlineData("", "audience")]
    [InlineData("issuer", null)]
    [InlineData("issuer", "")]
    public void GenerateTokenRejectsEmptyIssuerOrAudience(string? issuer, string? audience)
    {
        var options = CreateValidOptions() with { Issuer = issuer, Audience = audience };
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
        var options = CreateValidOptions() with { AccessTokenMinutes = accessTokenMinutes };
        var service = CreateService(options);

        var exception = Assert.Throws<InvalidOperationException>(() => service.GenerateToken(CreateRequest()));
        Assert.Contains("expiration", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static JwtTokenService CreateService(JwtOptions options) =>
        new(Options.Create(options), new FixedTimeProvider(FixedNow));

    private static JwtOptions CreateValidOptions() =>
        new()
        {
            Issuer = "NuamExchange.Tests",
            Audience = "NuamExchange.Tests.Client",
            SigningKey = "test-signing-key-with-at-least-32-characters",
            AccessTokenMinutes = 45,
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
                notBefore <= FixedNow.UtcDateTime && expires == FixedNow.AddMinutes(options.AccessTokenMinutes).UtcDateTime,
        };

        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        return handler.ValidateToken(accessToken, parameters, out _);
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
