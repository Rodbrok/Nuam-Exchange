using System.Reflection;
using System.Text.RegularExpressions;
using NuamExchange.Application.Classifications;
using NuamExchange.Application.Exceptions;
using NuamExchange.Domain.Classifications;
using Xunit;

namespace NuamExchange.Application.Tests;

public sealed class ClassificationServiceTests
{
    public static IEnumerable<object[]> InvalidRequests()
    {
        yield return [Valid() with { FiscalYear = 2023 }, "fiscalYear"];
        yield return [Valid() with { FiscalYear = 2101 }, "fiscalYear"];
        yield return [Valid() with { Market = "Derivados" }, "market"];
        yield return [Valid() with { Source = "Carga Archivo" }, "source"];
        yield return [Valid() with { Instrument = "" }, "instrument"];
        yield return [Valid() with { Instrument = "A" }, "instrument"];
        yield return [Valid() with { Instrument = new string('A', 31) }, "instrument"];
        yield return [Valid() with { Description = "" }, "description"];
        yield return [Valid() with { Description = "AB" }, "description"];
        yield return [Valid() with { Description = new string('D', 501) }, "description"];
        yield return [Valid() with { EventSequence = "" }, "eventSequence"];
        yield return [Valid() with { EventSequence = "EVT_001" }, "eventSequence"];
        yield return [Valid() with { EventSequence = new string('E', 31) }, "eventSequence"];
        yield return [Valid() with { UpdateFactor = 0m }, "updateFactor"];
        yield return [Valid() with { UpdateFactor = -1m }, "updateFactor"];
        yield return [Valid() with { UpdateFactor = 1.1234567m }, "updateFactor"];
        yield return [Valid() with { Amount = -0.01m }, "amount"];
        yield return [Valid() with { Amount = 10.123m }, "amount"];
        yield return [Valid() with { Status = (ClassificationStatus)999 }, "status"];
    }

    [Fact]
    public async Task ValidRequestCreatesClassification()
    {
        var service = new ClassificationService(new FakeClassificationRepository());
        var response = await service.CreateAsync(Valid(), CancellationToken.None);
        Assert.Equal("Calificación creada correctamente.", response.Message);
        Assert.Equal("Acciones", response.Classification.Market);
    }

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public async Task InvalidRequestThrowsValidationException(CreateClassificationRequest request, string field)
    {
        var service = new ClassificationService(new FakeClassificationRepository());
        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(request, CancellationToken.None));
        Assert.Contains(field, exception.Errors.Keys);
    }

    [Fact]
    public void RowVersionByteArrayConvertsToBase64()
    {
        var classification = Existing("CAL-2026-ROW");
        SetRowVersion(classification, [1, 2, 3, 4]);
        Assert.Equal("AQIDBA==", ClassificationMapper.ToDto(classification).RowVersion);
    }

    [Fact]
    public async Task ValidBase64RowVersionIsAcceptedForUpdate()
    {
        var existing = Existing("CAL-2026-001");
        SetRowVersion(existing, [1, 2, 3, 4]);
        var repository = new FakeClassificationRepository(existing);
        var service = new ClassificationService(repository);
        var response = await service.UpdateAsync(existing.Id, ValidUpdate(), "AQIDBA==", CancellationToken.None);
        Assert.Equal("Calificación actualizada correctamente.", response.Message);
    }

    [Fact]
    public async Task InvalidBase64RowVersionThrowsValidationException()
    {
        var existing = Existing("CAL-2026-001");
        var service = new ClassificationService(new FakeClassificationRepository(existing));
        await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(existing.Id, ValidUpdate(), "not-base64", CancellationToken.None));
    }

    [Fact]
    public async Task GeneratedIdUsesExpectedFormat()
    {
        var service = new ClassificationService(new FakeClassificationRepository());
        var response = await service.CreateAsync(Valid(), CancellationToken.None);
        Assert.Matches(new Regex("^CAL-2026-[0-9A-F]{8}$"), response.Classification.Id);
    }

    [Fact]
    public async Task GeneratedIdsAreDifferent()
    {
        var service = new ClassificationService(new FakeClassificationRepository());
        var first = await service.CreateAsync(Valid(), CancellationToken.None);
        var second = await service.CreateAsync(Valid() with { EventSequence = "EVT-002" }, CancellationToken.None);
        Assert.NotEqual(first.Classification.Id, second.Classification.Id);
    }

    [Fact]
    public async Task DuplicateCreateThrowsConflictException()
    {
        var service = new ClassificationService(new FakeClassificationRepository { Duplicate = true });
        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(Valid(), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateExcludesCurrentRecordWhenCheckingDuplicates()
    {
        var existing = Existing("CAL-2026-001");
        var repository = new FakeClassificationRepository(existing);
        var service = new ClassificationService(repository);
        await service.UpdateAsync(existing.Id, ValidUpdate(), null, CancellationToken.None);
        Assert.Equal(existing.Id, repository.LastDuplicateExcludeId);
    }

    [Fact]
    public async Task CopyDetectsDuplicate()
    {
        var existing = Existing("CAL-2026-001");
        var service = new ClassificationService(new FakeClassificationRepository(existing) { Duplicate = true });
        await Assert.ThrowsAsync<ConflictException>(() => service.CopyAsync(existing.Id, new CopyClassificationRequest(null, null, null), CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdMissingThrowsNotFoundException()
    {
        var service = new ClassificationService(new FakeClassificationRepository());
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync("missing", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteMissingThrowsNotFoundException()
    {
        var service = new ClassificationService(new FakeClassificationRepository());
        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync("missing", null, CancellationToken.None));
    }

    private static CreateClassificationRequest Valid() => new(
        "Acciones", "Manual", 2026, "NUAM-A", new DateOnly(2026, 1, 15), "Dividendo válido", "EVT-001", 1.123456m, 100.25m, ClassificationStatus.Vigente);

    private static UpdateClassificationRequest ValidUpdate() => new(
        "Acciones", "Manual", 2026, "NUAM-A", new DateOnly(2026, 1, 15), "Dividendo válido", "EVT-001", 1.123456m, 100.25m, ClassificationStatus.Vigente);

    private static Classification Existing(string id) => new(
        id, "Acciones", "Manual", 2026, "NUAM-A", new DateOnly(2026, 1, 15), "Dividendo válido", "EVT-001", 1m, 100m, ClassificationStatus.Vigente, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

    private static void SetRowVersion(Classification classification, byte[] rowVersion) =>
        typeof(Classification).GetProperty(nameof(Classification.RowVersion), BindingFlags.Instance | BindingFlags.Public)!.SetValue(classification, rowVersion);
}

internal sealed class FakeClassificationRepository(Classification? existing = null) : IClassificationRepository
{
    public bool Duplicate { get; init; }
    public string? LastDuplicateExcludeId { get; private set; }

    public Task<IReadOnlyList<Classification>> ListAsync(ClassificationListRequest request, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Classification>>([]);
    public Task<int> CountAsync(ClassificationListRequest request, CancellationToken cancellationToken) => Task.FromResult(0);
    public Task<Classification?> GetByIdAsync(string id, bool tracking, CancellationToken cancellationToken) => Task.FromResult(existing?.Id == id ? existing : null);
    public Task<ClassificationCatalogValues> GetCatalogsAsync(CancellationToken cancellationToken) => Task.FromResult(new ClassificationCatalogValues([]));
    public Task<bool> ExistsDuplicateAsync(int fiscalYear, string market, string instrument, DateOnly paymentDate, string eventSequence, string? excludeId, CancellationToken cancellationToken)
    {
        LastDuplicateExcludeId = excludeId;
        return Task.FromResult(Duplicate);
    }

    public Task AddAsync(Classification classification, CancellationToken cancellationToken) => Task.CompletedTask;
    public void Update(Classification classification, byte[]? originalRowVersion) { }
    public void Remove(Classification classification, byte[]? originalRowVersion) { }
    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
