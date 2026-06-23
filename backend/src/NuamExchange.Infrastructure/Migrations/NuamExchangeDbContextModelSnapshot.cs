using Microsoft.EntityFrameworkCore; using Microsoft.EntityFrameworkCore.Infrastructure; using NuamExchange.Infrastructure.Persistence;
#nullable disable
namespace NuamExchange.Infrastructure.Migrations;
[DbContext(typeof(NuamExchangeDbContext))] public partial class NuamExchangeDbContextModelSnapshot : ModelSnapshot { protected override void BuildModel(ModelBuilder modelBuilder) { modelBuilder.HasAnnotation("ProductVersion", "8.0.22"); } }
