using Microsoft.EntityFrameworkCore;
using NuamExchange.Domain.Classifications;

namespace NuamExchange.Infrastructure.Persistence;

public sealed class NuamExchangeDbContext(DbContextOptions<NuamExchangeDbContext> options) : DbContext(options)
{
    public DbSet<Classification> Classifications => Set<Classification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NuamExchangeDbContext).Assembly);

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (Database.IsInMemory())
        {
            foreach (var entry in ChangeTracker.Entries<Classification>().Where(e => e.State is EntityState.Added or EntityState.Modified))
            {
                entry.Property(x => x.RowVersion).CurrentValue = Guid.NewGuid().ToByteArray();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
