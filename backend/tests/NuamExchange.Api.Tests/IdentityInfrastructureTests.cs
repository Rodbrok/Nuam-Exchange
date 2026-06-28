using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NuamExchange.Application.Authentication;
using NuamExchange.Infrastructure.Identity;
using Xunit;

namespace NuamExchange.Api.Tests;

public sealed class IdentityInfrastructureTests
{
    [Fact]
    public async Task UserManagerAndRoleManagerCreateUserAndAssignAdministratorRole()
    {
        using var factory = CreateFactory();
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var unique = Guid.NewGuid().ToString("N");
        var email = $"identity-{unique}@example.test";
        const string password = "Valid!Pass123";

        var roleResult = await roleManager.CreateAsync(new IdentityRole(RoleNames.Administrador));
        Assert.True(roleResult.Succeeded, Describe(roleResult.Errors));

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = $"Usuario Identity {unique}",
        };

        var createResult = await userManager.CreateAsync(user, password);
        Assert.True(createResult.Succeeded, Describe(createResult.Errors));
        Assert.True(await userManager.CheckPasswordAsync(user, password));

        var addToRoleResult = await userManager.AddToRoleAsync(user, RoleNames.Administrador);
        Assert.True(addToRoleResult.Succeeded, Describe(addToRoleResult.Errors));
        Assert.True(await userManager.IsInRoleAsync(user, RoleNames.Administrador));
    }

    [Fact]
    public async Task WeakPasswordIsRejected()
    {
        using var factory = CreateFactory();
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var unique = Guid.NewGuid().ToString("N");
        var email = $"weak-{unique}@example.test";
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = $"Usuario Weak {unique}",
        };

        var result = await userManager.CreateAsync(user, "weak");

        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task DuplicateEmailIsRejected()
    {
        using var factory = CreateFactory();
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var unique = Guid.NewGuid().ToString("N");
        var email = $"duplicate-{unique}@example.test";
        const string password = "Valid!Pass123";

        var firstUser = CreateUser(email, $"Usuario Original {unique}");
        var secondUser = CreateUser(email, $"Usuario Duplicado {unique}");

        var firstResult = await userManager.CreateAsync(firstUser, password);
        var secondResult = await userManager.CreateAsync(secondUser, password);

        Assert.True(firstResult.Succeeded, Describe(firstResult.Errors));
        Assert.False(secondResult.Succeeded);
        Assert.Contains(secondResult.Errors, error => error.Code == nameof(IdentityErrorDescriber.DuplicateEmail));
    }

    private static TestingWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));

    private static ApplicationUser CreateUser(string email, string fullName) =>
        new()
        {
            UserName = email,
            Email = email,
            FullName = fullName,
        };

    private static string Describe(IEnumerable<IdentityError> errors) =>
        string.Join(", ", errors.Select(error => $"{error.Code}: {error.Description}"));
}
