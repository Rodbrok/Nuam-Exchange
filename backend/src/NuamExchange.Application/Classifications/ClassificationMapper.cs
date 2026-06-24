using NuamExchange.Domain.Classifications;

namespace NuamExchange.Application.Classifications;

public static class ClassificationMapper
{
    public static ClassificationDto ToDto(Classification classification) =>
        new(
            classification.Id,
            classification.Market,
            classification.Source,
            classification.FiscalYear,
            classification.Instrument,
            classification.PaymentDate,
            classification.Description,
            classification.EventSequence,
            classification.UpdateFactor,
            classification.Amount,
            classification.Status,
            Convert.ToBase64String(classification.RowVersion));
}
