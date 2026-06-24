using Microsoft.AspNetCore.Identity;

namespace NuamExchange.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<string>
{
    public ApplicationUser()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
