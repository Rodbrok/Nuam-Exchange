using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace NuamExchange.Api.Tests;

public sealed class JwtBearerAuthenticationTests
{
    private const string Issuer = "NuamExchange.Api.Tests.JwtBearer";
    private const string Audience = "NuamExchange.Api.Tests.Client";
    private const string SigningKey = "jwt-bearer-test-signing-key-32-plus-chars";

    [Fact]
    public void DefaultAuthenticateSchemeIsBearer()
    {
        using var factory = CreateFactory();
        var options = factory.Services.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, options.DefaultAuthenticateScheme);
    }

    [Fact]
    public void DefaultChallengeSchemeIsBearer()
    {
        using var factory = CreateFactory();
        var options = factory.Services.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, options.DefaultChallengeScheme);
    }

    [Fact]
    public void AuthorizationServiceIsRegistered()
    {
        using var factory = CreateFactory();

        Assert.NotNull(factory.Services.GetRequiredService<IAuthorizationService>());
    }

    [Fact]
    public void JwtBearerOptionsUseConfiguredIssuerAudienceAndSigningKey()
    {
        using var factory = CreateFactory();
        var parameters = GetTokenValidationParameters(factory);

        Assert.Equal(Issuer, parameters.ValidIssuer);
        Assert.Equal(Audience, parameters.ValidAudience);
        var signingKey = Assert.IsType<SymmetricSecurityKey>(parameters.IssuerSigningKey);
        Assert.Equal(Encoding.UTF8.GetBytes(SigningKey), signingKey.Key);
    }

    [Fact]
    public void JwtBearerOptionsEnableRequiredValidationRules()
    {
        using var factory = CreateFactory();
        var parameters = GetTokenValidationParameters(factory);

        Assert.True(parameters.ValidateIssuer);
        Assert.True(parameters.ValidateAudience);
        Assert.True(parameters.ValidateIssuerSigningKey);
        Assert.True(parameters.ValidateLifetime);
        Assert.Equal(TimeSpan.Zero, parameters.ClockSkew);
    }

    [Fact]
    public void JwtBearerOptionsUseExpectedClaimTypes()
    {
        using var factory = CreateFactory();
        var parameters = GetTokenValidationParameters(factory);

        Assert.Equal(ClaimTypes.Name, parameters.NameClaimType);
        Assert.Equal(ClaimTypes.Role, parameters.RoleClaimType);
    }

    [Fact]
    public void InvalidSigningKeyIsRejected()
    {
        using var factory = CreateFactory(new Dictionary<string, string?> { ["Jwt:SigningKey"] = "too-short" });
        var options = factory.Services.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();

        var exception = Assert.Throws<InvalidOperationException>(() => options.Get(JwtBearerDefaults.AuthenticationScheme));
        Assert.Contains("Jwt:SigningKey", exception.Message, StringComparison.Ordinal);
    }

    private static TestingWebApplicationFactory CreateFactory(IReadOnlyDictionary<string, string?>? configuration = null)
    {
        var values = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = Issuer,
            ["Jwt:Audience"] = Audience,
            ["Jwt:SigningKey"] = SigningKey,
            ["Jwt:AccessTokenMinutes"] = "30",
        };

        if (configuration is not null)
        {
            foreach (var item in configuration)
            {
                values[item.Key] = item.Value;
            }
        }

        return new TestingWebApplicationFactory(Guid.NewGuid().ToString("N"), values);
    }

    private static TokenValidationParameters GetTokenValidationParameters(TestingWebApplicationFactory factory) =>
        factory.Services.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;
}
