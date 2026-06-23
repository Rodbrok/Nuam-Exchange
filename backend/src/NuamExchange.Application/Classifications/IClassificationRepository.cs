using NuamExchange.Domain.Classifications;

namespace NuamExchange.Application.Classifications;

public interface IClassificationRepository
{
    Task<IReadOnlyList<Classification>> ListAsync(ClassificationListRequest request, CancellationToken ct);
    Task<int> CountAsync(ClassificationListRequest request, CancellationToken ct);
    Task<Classification?> GetByIdAsync(string id, bool tracking, CancellationToken ct);
    Task<ClassificationCatalogValues> GetCatalogsAsync(CancellationToken ct);
    Task<bool> ExistsDuplicateAsync(
        int fiscalYear,
        string market,
        string instrument,
        DateOnly paymentDate,
        string eventSequence,
        string? excludeId,
        CancellationToken ct);
    Task AddAsync(Classification classification, CancellationToken ct);
    void Update(Classification classification, byte[]? originalRowVersion);
    void Remove(Classification classification, byte[]? originalRowVersion);
    Task SaveChangesAsync(CancellationToken ct);
}
