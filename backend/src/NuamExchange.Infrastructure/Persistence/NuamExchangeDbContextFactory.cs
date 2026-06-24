using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NuamExchange.Infrastructure.Persistence;

public sealed class NuamExchangeDbContextFactory : IDesignTimeDbContextFactory<NuamExchangeDbContext>
{
    public NuamExchangeDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=NuamExchangeDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

        var builder = new DbContextOptionsBuilder<NuamExchangeDbContext>()
            .UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());

        return new NuamExchangeDbContext(builder.Options);
    }
}
