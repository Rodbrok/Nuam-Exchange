using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuamExchange.Application.Classifications;
using NuamExchange.Infrastructure.Classifications;
using NuamExchange.Infrastructure.Persistence;

namespace NuamExchange.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<NuamExchangeDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

        services.AddScoped<IClassificationRepository, ClassificationRepository>();

        return services;
    }
}
