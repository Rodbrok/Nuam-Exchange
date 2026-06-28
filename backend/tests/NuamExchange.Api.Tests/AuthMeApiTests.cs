using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using NuamExchange.Api.Authentication;
using NuamExchange.Application.Authentication;
using Swashbuckle.AspNetCore.Swagger;
using Xunit;

namespace NuamExchange.Api.Tests;

public sealed class AuthMeApiTests
{
    private static readonly string[] ExpectedRoles = ["admin", "reader"];

    [Fact]
    public async Task MeWithoutTokenReturnsUnauthorized()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task MeWithValidTokenReturnsCurrentUserClaims()
    {
        using var factory = CreateFactory();
        var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();
        var token = tokenService.GenerateToken(new JwtTokenRequest(
            "user-123",
            "user@example.com",
            "Test User",
            ["admin", "reader", "admin", ""]));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var response = await client.GetAsync("/api/v1/auth/me");
        var body = await response.Content.ReadFromJsonAsync<CurrentUserResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("user-123", body.UserId);
        Assert.Equal("user@example.com", body.Email);
        Assert.Equal("Test User", body.FullName);
        Assert.Equal(ExpectedRoles, body.Roles);
    }

    [Fact]
    public void SwaggerContainsBearerSchemeAndMarksOnlyProtectedOperations()
    {
        using var factory = CreateFactory();
        var swagger = factory.Services.GetRequiredService<ISwaggerProvider>().GetSwagger("v1");

        Assert.True(swagger.Components.SecuritySchemes.ContainsKey("Bearer"));
        var meOperation = swagger.Paths["/api/v1/auth/me"].Operations[Microsoft.OpenApi.Models.OperationType.Get];
        Assert.Contains(meOperation.Security, requirement => requirement.Keys.Any(scheme => scheme.Reference?.Id == "Bearer"));
        Assert.True(meOperation.Responses.ContainsKey("401"));

        var classificationsOperation = swagger.Paths["/api/v1/classifications"].Operations[Microsoft.OpenApi.Models.OperationType.Get];
        Assert.True(classificationsOperation.Security is null || classificationsOperation.Security.Count == 0);
    }

    [Fact]
    public async Task ExistingPublicClassificationsAndHealthEndpointsRemainPublic()
    {
        using var factory = CreateFactory();
        var client = factory.CreateClient();

        var classifications = await client.GetAsync("/api/v1/classifications");
        var live = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, classifications.StatusCode);
        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
    }

    private static TestingWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
