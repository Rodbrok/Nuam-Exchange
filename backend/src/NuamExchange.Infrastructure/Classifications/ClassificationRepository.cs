using Microsoft.EntityFrameworkCore;
using NuamExchange.Application.Classifications;
using NuamExchange.Application.Exceptions;
using NuamExchange.Domain.Classifications;
using NuamExchange.Infrastructure.Persistence;

namespace NuamExchange.Infrastructure.Classifications;

public sealed class ClassificationRepository(NuamExchangeDbContext db) : IClassificationRepository
{
    public async Task<IReadOnlyList<Classification>> ListAsync(ClassificationListRequest request, CancellationToken ct) =>
        await ApplySort(ApplyFilters(db.Classifications.AsNoTracking(), request), request)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

    public Task<int> CountAsync(ClassificationListRequest request, CancellationToken ct) =>
        ApplyFilters(db.Classifications.AsNoTracking(), request).CountAsync(ct);

    public Task<Classification?> GetByIdAsync(string id, bool tracking, CancellationToken ct) =>
        (tracking ? db.Classifications : db.Classifications.AsNoTracking()).FirstOrDefaultAsync(classification => classification.Id == id, ct);

    public async Task<ClassificationCatalogValues> GetCatalogsAsync(CancellationToken ct) =>
        new(await db.Classifications
            .AsNoTracking()
            .Select(classification => classification.FiscalYear)
            .Distinct()
            .OrderBy(fiscalYear => fiscalYear)
            .ToListAsync(ct));

    public Task<bool> ExistsDuplicateAsync(
        int fiscalYear,
        string market,
        string instrument,
        DateOnly paymentDate,
        string eventSequence,
        string? excludeId,
        CancellationToken ct) =>
        db.Classifications
            .AsNoTracking()
            .AnyAsync(
                classification => classification.FiscalYear == fiscalYear
                    && classification.Market == market
                    && classification.Instrument == instrument
                    && classification.PaymentDate == paymentDate
                    && classification.EventSequence == eventSequence
                    && (excludeId == null || classification.Id != excludeId),
                ct);

    public Task AddAsync(Classification classification, CancellationToken ct) =>
        db.Classifications.AddAsync(classification, ct).AsTask();

    public void Update(Classification classification, byte[]? originalRowVersion)
    {
        if (originalRowVersion is not null)
        {
            db.Entry(classification).Property(entity => entity.RowVersion).OriginalValue = originalRowVersion;
        }
    }

    public void Remove(Classification classification, byte[]? originalRowVersion)
    {
        if (originalRowVersion is not null)
        {
            db.Entry(classification).Property(entity => entity.RowVersion).OriginalValue = originalRowVersion;
        }

        db.Classifications.Remove(classification);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException("La calificación fue modificada por otro proceso.");
        }
    }

    private static IQueryable<Classification> ApplyFilters(IQueryable<Classification> query, ClassificationListRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(classification => classification.Instrument.Contains(request.Search)
                || classification.Description.Contains(request.Search)
                || classification.EventSequence.Contains(request.Search));
        }

        if (!string.IsNullOrWhiteSpace(request.Market))
        {
            query = query.Where(classification => classification.Market == request.Market);
        }

        if (!string.IsNullOrWhiteSpace(request.Source))
        {
            query = query.Where(classification => classification.Source == request.Source);
        }

        if (request.FiscalYear is not null)
        {
            query = query.Where(classification => classification.FiscalYear == request.FiscalYear);
        }

        if (request.Status is not null)
        {
            query = query.Where(classification => classification.Status == request.Status);
        }

        return query;
    }

    private static IQueryable<Classification> ApplySort(IQueryable<Classification> query, ClassificationListRequest request) =>
        (request.SortBy, request.SortDirection) switch
        {
            ("fiscalYear", "asc") => query.OrderBy(classification => classification.FiscalYear),
            ("fiscalYear", _) => query.OrderByDescending(classification => classification.FiscalYear),
            ("instrument", "asc") => query.OrderBy(classification => classification.Instrument),
            ("instrument", _) => query.OrderByDescending(classification => classification.Instrument),
            ("amount", "asc") => query.OrderBy(classification => classification.Amount),
            ("amount", _) => query.OrderByDescending(classification => classification.Amount),
            ("status", "asc") => query.OrderBy(classification => classification.Status),
            ("status", _) => query.OrderByDescending(classification => classification.Status),
            ("paymentDate", "asc") => query.OrderBy(classification => classification.PaymentDate),
            _ => query.OrderByDescending(classification => classification.PaymentDate),
        };
}
