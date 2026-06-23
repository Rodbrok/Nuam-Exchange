using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuamExchange.Domain.Classifications;

namespace NuamExchange.Infrastructure.Persistence;

public sealed class ClassificationConfiguration : IEntityTypeConfiguration<Classification>
{
    public void Configure(EntityTypeBuilder<Classification> builder)
    {
        builder.ToTable("Classifications");

        builder.HasKey(classification => classification.Id);

        builder.Property(classification => classification.Id)
            .HasMaxLength(40)
            .ValueGeneratedNever();

        builder.Property(classification => classification.Market)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(classification => classification.Source)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(classification => classification.Instrument)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(classification => classification.PaymentDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(classification => classification.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(classification => classification.EventSequence)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(classification => classification.UpdateFactor)
            .HasColumnType("decimal(18,6)")
            .IsRequired();

        builder.Property(classification => classification.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(classification => classification.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(classification => classification.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(classification => classification.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(classification => classification.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(classification => new
            {
                classification.FiscalYear,
                classification.Market,
                classification.Instrument,
                classification.PaymentDate,
                classification.EventSequence,
            })
            .IsUnique();

        builder.HasIndex(classification => classification.FiscalYear);
        builder.HasIndex(classification => classification.Market);
        builder.HasIndex(classification => classification.Status);
        builder.HasIndex(classification => classification.PaymentDate);

        builder.HasData(ClassificationSeed.Items);
    }
}
