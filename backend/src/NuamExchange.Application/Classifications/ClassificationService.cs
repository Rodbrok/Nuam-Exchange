using System.Security.Cryptography;
using System.Text.RegularExpressions;
using NuamExchange.Application.Common;
using NuamExchange.Application.Exceptions;
using NuamExchange.Domain.Classifications;

namespace NuamExchange.Application.Classifications;

public interface IClassificationService
{
    Task<PaginatedResponse<ClassificationDto>> ListAsync(ClassificationListRequest r, CancellationToken ct);
    Task<ClassificationCatalogsDto> GetCatalogsAsync(CancellationToken ct);
    Task<ClassificationDto> GetByIdAsync(string id, CancellationToken ct);
    Task<ClassificationWriteResponse> CreateAsync(CreateClassificationRequest r, CancellationToken ct);
    Task<ClassificationWriteResponse> UpdateAsync(string id, UpdateClassificationRequest r, string? ifMatch, CancellationToken ct);
    Task<ClassificationWriteResponse> CopyAsync(string id, CopyClassificationRequest r, CancellationToken ct);
    Task DeleteAsync(string id, string? ifMatch, CancellationToken ct);
}

public sealed class ClassificationService(IClassificationRepository repo) : IClassificationService
{
    private static readonly string[] Markets = ["Acciones", "Renta Fija", "Fondos"];
    private static readonly string[] Sources = ["Manual", "Carga X Factor", "Carga X Monto"];
    private static readonly string[] Sorts = ["fiscalYear", "instrument", "paymentDate", "amount", "status"];
    private static readonly int[] AllowedPageSizes = [5, 10, 20];

    public async Task<PaginatedResponse<ClassificationDto>> ListAsync(ClassificationListRequest r, CancellationToken ct)
    {
        ValidateList(r);
        var items = (await repo.ListAsync(r, ct)).Select(ClassificationMapper.ToDto).ToList();
        var total = await repo.CountAsync(r, ct);
        var totalPages = total == 0 ? 0 : (int)Math.Ceiling((double)total / r.PageSize);

        return new PaginatedResponse<ClassificationDto>(items, r.Page, r.PageSize, total, totalPages);
    }

    public async Task<ClassificationCatalogsDto> GetCatalogsAsync(CancellationToken ct)
    {
        var catalogs = await repo.GetCatalogsAsync(ct);
        var fiscalYears = catalogs.FiscalYears.Count == 0 ? [DateTimeOffset.UtcNow.Year] : catalogs.FiscalYears;

        return new ClassificationCatalogsDto(Markets, Sources, fiscalYears, Enum.GetValues<ClassificationStatus>());
    }

    public async Task<ClassificationDto> GetByIdAsync(string id, CancellationToken ct) =>
        ClassificationMapper.ToDto(await Find(id, tracking: false, ct));

    public async Task<ClassificationWriteResponse> CreateAsync(CreateClassificationRequest r, CancellationToken ct)
    {
        ValidateWrite(r);
        await EnsureNoDuplicate(r.FiscalYear, r.Market, r.Instrument, r.PaymentDate, r.EventSequence, excludeId: null, ct);

        var now = DateTimeOffset.UtcNow;
        var entity = new Classification(
            NewId(r.FiscalYear),
            r.Market,
            r.Source,
            r.FiscalYear,
            r.Instrument,
            r.PaymentDate,
            r.Description,
            r.EventSequence,
            r.UpdateFactor,
            r.Amount,
            r.Status,
            now,
            now);

        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);

        return new ClassificationWriteResponse(ClassificationMapper.ToDto(entity), "Calificación creada correctamente.");
    }

    public async Task<ClassificationWriteResponse> UpdateAsync(string id, UpdateClassificationRequest r, string? ifMatch, CancellationToken ct)
    {
        ValidateWrite(r);
        var entity = await Find(id, tracking: true, ct);
        var rowVersion = ParseIfMatch(ifMatch);

        if (rowVersion is not null && !entity.RowVersion.SequenceEqual(rowVersion))
        {
            throw new ConcurrencyException("La calificación fue modificada por otro proceso.");
        }

        await EnsureNoDuplicate(r.FiscalYear, r.Market, r.Instrument, r.PaymentDate, r.EventSequence, id, ct);
        entity.Update(
            r.Market,
            r.Source,
            r.FiscalYear,
            r.Instrument,
            r.PaymentDate,
            r.Description,
            r.EventSequence,
            r.UpdateFactor,
            r.Amount,
            r.Status,
            DateTimeOffset.UtcNow);

        repo.Update(entity, rowVersion);
        await repo.SaveChangesAsync(ct);

        return new ClassificationWriteResponse(ClassificationMapper.ToDto(entity), "Calificación actualizada correctamente.");
    }

    public async Task<ClassificationWriteResponse> CopyAsync(string id, CopyClassificationRequest r, CancellationToken ct)
    {
        var original = await Find(id, tracking: false, ct);
        var paymentDate = r.PaymentDate ?? original.PaymentDate;
        var description = string.IsNullOrWhiteSpace(r.Description) ? original.Description : r.Description;
        var eventSequence = string.IsNullOrWhiteSpace(r.EventSequence) ? original.EventSequence : r.EventSequence;
        var request = new CreateClassificationRequest(
            original.Market,
            original.Source,
            original.FiscalYear,
            original.Instrument,
            paymentDate,
            description,
            eventSequence,
            original.UpdateFactor,
            original.Amount,
            original.Status);

        ValidateWrite(request);
        await EnsureNoDuplicate(request.FiscalYear, request.Market, request.Instrument, request.PaymentDate, request.EventSequence, excludeId: null, ct);

        var now = DateTimeOffset.UtcNow;
        var entity = new Classification(
            NewId(request.FiscalYear),
            request.Market,
            request.Source,
            request.FiscalYear,
            request.Instrument,
            request.PaymentDate,
            request.Description,
            request.EventSequence,
            request.UpdateFactor,
            request.Amount,
            request.Status,
            now,
            now);

        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);

        return new ClassificationWriteResponse(ClassificationMapper.ToDto(entity), "Calificación copiada correctamente.");
    }

    public async Task DeleteAsync(string id, string? ifMatch, CancellationToken ct)
    {
        var entity = await Find(id, tracking: true, ct);
        var rowVersion = ParseIfMatch(ifMatch);

        if (rowVersion is not null && !entity.RowVersion.SequenceEqual(rowVersion))
        {
            throw new ConcurrencyException("La calificación fue modificada por otro proceso.");
        }

        repo.Remove(entity, rowVersion);
        await repo.SaveChangesAsync(ct);
    }

    private static string NewId(int year) => $"CAL-{year}-{RandomNumberGenerator.GetHexString(8).ToUpperInvariant()}";

    private static byte[]? ParseIfMatch(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim().Trim('"');

        try
        {
            return Convert.FromBase64String(normalized);
        }
        catch (FormatException)
        {
            throw new ValidationException(
                "If-Match inválido.",
                new Dictionary<string, string[]> { { "If-Match", ["El encabezado If-Match debe contener RowVersion en Base64."] } });
        }
    }

    private async Task<Classification> Find(string id, bool tracking, CancellationToken ct) =>
        await repo.GetByIdAsync(id, tracking, ct) ?? throw new NotFoundException("La calificación solicitada no existe.");

    private async Task EnsureNoDuplicate(
        int fiscalYear,
        string market,
        string instrument,
        DateOnly paymentDate,
        string eventSequence,
        string? excludeId,
        CancellationToken ct)
    {
        if (await repo.ExistsDuplicateAsync(fiscalYear, market, instrument, paymentDate, eventSequence, excludeId, ct))
        {
            throw new ConflictException("Ya existe una calificación con la misma combinación de ejercicio, mercado, instrumento, fecha de pago y secuencia.");
        }
    }

    private static void ValidateList(ClassificationListRequest r)
    {
        var errors = new Dictionary<string, string[]>();

        if (r.Page < 1)
        {
            errors["page"] = ["La página debe ser mayor o igual a 1."];
        }

        if (!AllowedPageSizes.Contains(r.PageSize))
        {
            errors["pageSize"] = ["El tamaño de página permitido es 5, 10 o 20."];
        }

        if (!Sorts.Contains(r.SortBy))
        {
            errors["sortBy"] = ["El campo de ordenamiento no es válido."];
        }

        if (r.SortDirection is not ("asc" or "desc"))
        {
            errors["sortDirection"] = ["La dirección debe ser asc o desc."];
        }

        if (errors.Count > 0)
        {
            throw new ValidationException("La solicitud contiene errores de validación.", errors);
        }
    }

    private static void ValidateWrite(CreateClassificationRequest r) =>
        Validate(r.Market, r.Source, r.FiscalYear, r.Instrument, r.PaymentDate, r.Description, r.EventSequence, r.UpdateFactor, r.Amount, r.Status);

    private static void ValidateWrite(UpdateClassificationRequest r) =>
        Validate(r.Market, r.Source, r.FiscalYear, r.Instrument, r.PaymentDate, r.Description, r.EventSequence, r.UpdateFactor, r.Amount, r.Status);

    private static void Validate(
        string market,
        string source,
        int fiscalYear,
        string instrument,
        DateOnly _,
        string description,
        string eventSequence,
        decimal updateFactor,
        decimal amount,
        ClassificationStatus status)
    {
        var errors = new Dictionary<string, string[]>();

        if (!Markets.Contains(market))
        {
            errors["market"] = ["El mercado no es válido."];
        }

        if (!Sources.Contains(source))
        {
            errors["source"] = ["El origen no es válido."];
        }

        if (fiscalYear < 2024 || fiscalYear > 2100)
        {
            errors["fiscalYear"] = ["El ejercicio debe estar entre 2024 y 2100."];
        }

        if (string.IsNullOrWhiteSpace(instrument) || instrument.Length < 2 || instrument.Length > 30)
        {
            errors["instrument"] = ["El instrumento debe tener entre 2 y 30 caracteres."];
        }

        if (string.IsNullOrWhiteSpace(description) || description.Length < 3 || description.Length > 500)
        {
            errors["description"] = ["La descripción debe tener entre 3 y 500 caracteres."];
        }

        if (string.IsNullOrWhiteSpace(eventSequence) || eventSequence.Length < 3 || eventSequence.Length > 30 || !Regex.IsMatch(eventSequence, "^[A-Za-z0-9-]+$"))
        {
            errors["eventSequence"] = ["La secuencia debe tener entre 3 y 30 caracteres y solo letras, números o guion."];
        }

        if (updateFactor <= 0 || GetDecimalScale(updateFactor) > 6)
        {
            errors["updateFactor"] = ["El factor debe ser mayor que cero y tener máximo 6 decimales."];
        }

        if (amount < 0 || GetDecimalScale(amount) > 2)
        {
            errors["amount"] = ["El monto debe ser mayor o igual a cero y tener máximo 2 decimales."];
        }

        if (!Enum.IsDefined(status))
        {
            errors["status"] = ["El estado no es válido."];
        }

        if (errors.Count > 0)
        {
            throw new ValidationException("La solicitud contiene errores de validación.", errors);
        }
    }

    private static int GetDecimalScale(decimal value)
    {
        var bits = decimal.GetBits(value);
        return (bits[3] >> 16) & 0xFF;
    }
}
