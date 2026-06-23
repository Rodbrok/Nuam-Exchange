using NuamExchange.Domain.Classifications;
namespace NuamExchange.Application.Classifications;
public static class ClassificationMapper{ public static ClassificationDto ToDto(Classification c)=>new(c.Id,c.Market,c.Source,c.FiscalYear,c.Instrument,c.PaymentDate,c.Description,c.EventSequence,c.UpdateFactor,c.Amount,c.Status,Convert.ToBase64String(c.RowVersion)); }
