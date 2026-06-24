using System.ComponentModel.DataAnnotations;
using NuamExchange.Domain.Classifications;

namespace NuamExchange.Application.Classifications;

public sealed record ClassificationDto(
    string Id,
    string Market,
    string Source,
    int FiscalYear,
    string Instrument,
    DateOnly PaymentDate,
    string Description,
    string EventSequence,
    decimal UpdateFactor,
    decimal Amount,
    ClassificationStatus Status,
    string RowVersion);

public sealed record ClassificationListRequest(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? Market = null,
    string? Source = null,
    int? FiscalYear = null,
    ClassificationStatus? Status = null,
    string SortBy = "paymentDate",
    string SortDirection = "desc");

public sealed record ClassificationCatalogsDto(
    IReadOnlyList<string> Markets,
    IReadOnlyList<string> Sources,
    IReadOnlyList<int> FiscalYears,
    IReadOnlyList<ClassificationStatus> Statuses);

public sealed record CreateClassificationRequest(
    [Required] string Market,
    [Required] string Source,
    int FiscalYear,
    [Required] string Instrument,
    DateOnly PaymentDate,
    [Required] string Description,
    [Required] string EventSequence,
    decimal UpdateFactor,
    decimal Amount,
    ClassificationStatus Status);

public sealed record UpdateClassificationRequest(
    [Required] string Market,
    [Required] string Source,
    int FiscalYear,
    [Required] string Instrument,
    DateOnly PaymentDate,
    [Required] string Description,
    [Required] string EventSequence,
    decimal UpdateFactor,
    decimal Amount,
    ClassificationStatus Status);

public sealed record CopyClassificationRequest(
    DateOnly? PaymentDate,
    string? Description,
    string? EventSequence);

public sealed record ClassificationWriteResponse(
    ClassificationDto Classification,
    string Message);

public sealed record ClassificationCatalogValues(IReadOnlyList<int> FiscalYears);
