using Microsoft.EntityFrameworkCore;
using NuamExchange.Domain.Classifications;

namespace NuamExchange.Infrastructure.Persistence;

public sealed class NuamExchangeDbContext(DbContextOptions<NuamExchangeDbContext> options) : DbContext(options)
{
    private const string InMemoryProviderName = "Microsoft.EntityFrameworkCore.InMemory";

    public DbSet<Classification> Classifications => Set<Classification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NuamExchangeDbContext).Assembly);

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (IsUsingInMemoryProvider())
        {
            foreach (var entry in ChangeTracker.Entries<Classification>().Where(entry => entry.State is EntityState.Added or EntityState.Modified))
            {
                entry.Property(classification => classification.RowVersion).CurrentValue = Guid.NewGuid().ToByteArray();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private bool IsUsingInMemoryProvider() =>
        string.Equals(Database.ProviderName, InMemoryProviderName, StringComparison.Ordinal);
}
