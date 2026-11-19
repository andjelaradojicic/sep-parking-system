using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Psp.Api.Domain;

namespace Psp.Api.Data.Configurations;

public class PspTransactionConfiguration : IEntityTypeConfiguration<PspTransaction>
{
    public void Configure(EntityTypeBuilder<PspTransaction> b)
    {
        b.ToTable("psp_transactions");
        b.HasKey(x => x.Id);

        b.Property(x => x.MerchantOrderId).HasMaxLength(64);
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();
        b.Property(x => x.Description).HasMaxLength(140);
        b.Property(x => x.PaymentMethod).HasMaxLength(16);

        b.Property(x => x.Status)
            .HasMaxLength(32)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<PspTransactionStatus>(v, true));

        b.HasOne(x => x.Merchant)
            .WithMany()
            .HasForeignKey(x => x.MerchantId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.MerchantId, x.MerchantOrderId }).IsUnique();

        b.Property<uint>("xmin")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion();
    }
}